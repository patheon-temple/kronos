﻿using Asp.Versioning;
using Kronos.WebAPI.Kronos.Domain;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Kronos.WebAPI.Kronos;

public static class ServiceInstaller
{
    public static void Install(IServiceCollection services)
    {
        
// Add services to the container.
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers
                    // "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;

                    options.Policies.Sunset(0.9)
                        .Effective(DateTimeOffset.Now.AddDays(60))
                        .Link("policy.html")
                        .Title("Versioning Policy")
                        .Type("text/html");
                })
            .AddApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                })
            // this enables binding ApiVersion as a endpoint callback parameter. if you don't use it, then
            // you should remove this configuration.
            .EnableApiVersionBinding();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen( options =>
        {
            options.AddServer(new OpenApiServer
            {
                Description = "Localhost HTTPS",
                Url = "https://localhost:7115"
            });
            options.AddServer(new OpenApiServer
            {
                Description = "Localhost HTTP",
                Url = "http://localhost:5108"
            });
            options.AddServer(new OpenApiServer
            {
                Description = "Production",
                Url = "https://pantheon.obert.cz"
            });
            options.DocumentFilter<TagDescriptionsDocumentFilter>();
            options.SchemaFilter<EnumSchemaFilter>();
            options.OperationFilter<SwaggerDefaultValues>();
        });
        services.AddOptions<ServiceDiscovery>()
            .BindConfiguration("Kronos:Discovery")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}