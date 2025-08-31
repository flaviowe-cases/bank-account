using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Bank.Commons.Api;

public class OpenApiDocumentTransformer
    (ApiConfiguration apiConfiguration): IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document, 
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo()
        {
            Title = apiConfiguration.Title,
            Version = "1.0",
            Description = apiConfiguration.Description,
            License = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            },
            Contact = new OpenApiContact()
            {
                Name = "Luis Flavio Carvalho Alves",
                Email = "flaviowe@hotmail.com",
                Url = new Uri("https://www.linkedin.com/in/flavio-tecnologiadainformacao"),
            }

        };
        return Task.CompletedTask;
    }
}