using System.Net;
using System.Text;

namespace WeatherApp.Tests.TestUtils;

public class MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
{
    public static MockHttpHandler CreateJson(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new MockHttpHandler(_ =>
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        });
    }

    public static MockHttpHandler CreateFailure(HttpStatusCode statusCode)
    {
        return new MockHttpHandler(_ =>
        {
            return new HttpResponseMessage(statusCode);
        });
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(handler(request));
    }
}