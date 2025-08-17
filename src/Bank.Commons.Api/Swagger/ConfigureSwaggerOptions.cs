using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bank.Commons.Api.Swagger;

public class ConfigureSwaggerOptions(
    ApiConfiguration apiConfiguration,
    IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions> 
{
    private readonly ApiConfiguration _apiConfiguration = apiConfiguration;
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion( description ));
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        return new OpenApiInfo()
        {
            Title = _apiConfiguration.Title,
            Version = description.ApiVersion.ToString(),
            Description = _apiConfiguration.Description,
            Contact = new OpenApiContact() {Name = "Luis Flavio", Email = "flaviowe@hotmail.com"}, 
            License = new OpenApiLicense() { Name = "MIT", Url = new Uri( "https://opensource.org/licenses/MIT" ) }
        };
    }
}