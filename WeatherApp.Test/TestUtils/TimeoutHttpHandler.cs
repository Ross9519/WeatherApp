namespace WeatherApp.Tests.TestUtils;

public class TimeoutHttpHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        throw new TaskCanceledException("Simulated timeout");
    }
}