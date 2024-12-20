using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Kronos.WebAPI;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.WebApi;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
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

    var cryptoData = builder.Configuration
        .GetRequiredSection(GlobalDefinitions.ConfigurationKeys.HermesConfiguration)
        .Get<JwtConfig>()?.ToTokenCryptoData() ?? throw new NullReferenceException("No Jwt section");

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
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = GlobalDefinitions.Jwt.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(cryptoData.SigningKey),
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            TokenDecryptionKey = new SymmetricSecurityKey(cryptoData.EncryptionKey),
            ValidAudience = cryptoData.EntityId.ToString("N")
        };
    });
    builder.Services.AddHealthChecks();
    builder.Services.AddScoped<PantheonRequestContext>();
    builder.Services.AddSerilog();
    Kronos.WebAPI.Kronos.ServiceInstaller.Install(services);
    Kronos.WebAPI.Hermes.ServiceInstaller.Install(services, builder.Configuration);
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
    IdentityModelEventSource.ShowPII = true;
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