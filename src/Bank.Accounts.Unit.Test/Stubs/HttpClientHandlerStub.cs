using System.Net;

namespace Bank.Accounts.Unit.Test.Stubs;

public class HttpClientHandlerStub(
    HttpStatusCode statusCode, string responseBody = "") : HttpClientHandler
{
    private readonly HttpStatusCode _statusCode = statusCode;
    private readonly string _responseBody = responseBody;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage()
        {
            StatusCode = _statusCode,
            Content = new StringContent(_responseBody)
        };
        
        return Task.FromResult(response);   
    }
    
}