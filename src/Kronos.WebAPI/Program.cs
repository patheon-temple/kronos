using System.Text;
using FluentValidation;
using Kronos.WebAPI.Hermes.SDK;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddValidatorsFromAssemblyContaining<Program>();
services.AddFluentValidationRulesToSwagger();
services.AddCors();
var jwtOptionsSection = builder.Configuration
    .GetRequiredSection("Jwt")
    .Get<Kronos.WebAPI.Hermes.WebApi.JwtConfig>() ?? throw new NullReferenceException("No Jwt section");

services.AddAuthorization().AddAuthentication(
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

Kronos.WebAPI.Kronos.ServiceInstaller.Install(services);
Kronos.WebAPI.Hermes.ServiceInstaller.Install(services);
Kronos.WebAPI.Athena.ServiceInstaller.Install(services, builder.Configuration);

var app = builder.Build();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
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

app.UsePantheonRequestContext();
Kronos.WebAPI.Athena.WebApi.Endpoints.Register(app);
Kronos.WebAPI.Hermes.WebApi.Endpoints.Register(app);
Kronos.WebAPI.Kronos.WebApi.Endpoints.Register(app);
app.Run();