using Bank.Commons.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;

namespace Bank.Commons.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseCommonsApi(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        app.MapOpenApi();
        app.MapScalarApiReference();
        return app;
    }
}