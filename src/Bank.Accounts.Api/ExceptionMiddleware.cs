using Bank.Commons.Applications.Factories.Results;

namespace Bank.Accounts.Api;

public class ExceptionMiddleware(
    ILogger<ExceptionMiddleware> logger,
    RequestDelegate next)
{
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception on middleware {Message}",  e.Message);

            var failures = new List<ResultFail>()
            {
                new()
                {
                    Code = "UNHANDLED_EXCEPTION",
                    Message = e.Message
                },
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(failures);
        }
    }
}