using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Kronos.WebAPI;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.WebApi;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Grayscale)
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    services.AddHostedService<AutoEfMigrationsHostedService>();
    services.AddValidatorsFromAssemblyContaining<Program>();
    services.AddFluentValidationRulesToSwagger();
    services.AddCors();

    var jwtOptionsSection = builder.Configuration
        .GetRequiredSection("Jwt")
        .Get<JwtConfig>() ?? throw new NullReferenceException("No Jwt section");

    services.AddAuthorization(o =>
    {
        o.AddPolicy(GlobalDefinitions.Policies.SuperUser,
            policyBuilder => policyBuilder.RequireRole(GlobalDefinitions.Scopes.Superuser));
    }).AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwtOptions =>
    {
        if (string.IsNullOrWhiteSpace(jwtOptionsSection.Key)) throw new NullReferenceException("Jwt:Key is null");
        var key = Encoding.UTF8.GetBytes(jwtOptionsSection.Key);

        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptionsSection.Issuer,
            ValidAudience = jwtOptionsSection.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // RequireExpirationTime = true,
        };
    });
    builder.Services.AddHealthChecks();
    builder.Services.AddScoped<PantheonRequestContext>();
    builder.Services.AddSerilog();
    Kronos.WebAPI.Kronos.ServiceInstaller.Install(services);
    Kronos.WebAPI.Hermes.ServiceInstaller.Install(services);
    Kronos.WebAPI.Athena.ServiceInstaller.Install(services, builder.Configuration);

    var app = builder.Build();
    app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseSwagger();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerUI(
            options =>
            {
                var descriptions = app.DescribeApiVersions();

                // build a swagger endpoint for each discovered API version
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/healthz");

    app.UsePantheonRequestContext();

    Kronos.WebAPI.Athena.WebApi.Endpoints.Register(app);
    Kronos.WebAPI.Hermes.WebApi.Endpoints.Register(app);
    Kronos.WebAPI.Kronos.WebApi.Endpoints.Register(app);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}