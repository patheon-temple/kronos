using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Zeus.Endpoints;
using Zeus.Interop.Requests;
using Zeus.Interop.Responses;
using Zeus.Services;
using Zeus.Swagger;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

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
services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
services.AddScoped<TokenService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( options =>
    {
        var descriptions = app.DescribeApiVersions();

        // build a swagger endpoint for each discovered API version
        foreach ( var description in descriptions )
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint( url, name );
        }
    } );
}
var tokens = app.NewVersionedApi( "tokens" );
var tokensV1 = tokens.MapGroup( "/api/tokens" )
    .HasApiVersion( 1.0 );

tokensV1
    .MapPost("/", AuthenticationEndpoints.Post)
    .Accepts<AuthenticationPostRequest>("application/json")
    .Produces<AuthenticationSuccessfulResponse>()
    .Produces(400)
    .ProducesValidationProblem()
    .MapToApiVersion(1.0);

app.Run();