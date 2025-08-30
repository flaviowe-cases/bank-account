using Asp.Versioning.ApiExplorer;
using Bank.Commons.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bank.Commons.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCommonsApi(this WebApplication app, bool showSwagger = false)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        if (!app.Environment.IsDevelopment() && !showSwagger) 
            return app;
        
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToLowerInvariant());
        });
        
        return app;
    }
}