var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

Kronos.WebAPI.Kronos.ServiceInstaller.Install(services);
Kronos.WebAPI.Hermes.ServiceInstaller.Install(services);
Kronos.WebAPI.Athena.ServiceInstaller.Install(services, builder.Configuration);

var app = builder.Build();
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

Kronos.WebAPI.Athena.WebApi.Endpoints.Register(app);
Kronos.WebAPI.Hermes.WebApi.Endpoints.Register(app);
Kronos.WebAPI.Kronos.WebApi.Endpoints.Register(app);
app.Run();