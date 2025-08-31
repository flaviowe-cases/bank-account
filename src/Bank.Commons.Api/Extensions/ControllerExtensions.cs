using Microsoft.AspNetCore.Mvc;

namespace Bank.Commons.Api.Extensions;

public static class ControllerExtensions
{
    public static IActionResult Created<T>(
        this Controller controller, Guid id, T content)
    {
        var uri = controller.Request.Path
            .ToString()
            .TrimEnd('/');
        return controller.Created($"{uri}/{id}", content);
    }
}