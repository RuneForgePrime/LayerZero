using FluentAssertions;
using LayerZero.Tools.Configs.LeyLine;
using LayerZero.Tools.Managements;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LayerZero.Test.Tools.Management
{

    public class Response1
    {
        public string Message { get; set; }
    }
    public class LeyLineCallerTests
    {
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handlerFunc;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handlerFunc)
            {
                _handlerFunc = handlerFunc;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
                => _handlerFunc(request);
        }

        [Fact]
        public async Task GetAsync_ShouldParseJsonResponse()
        {
            var testObject = new { Message = "Hello" };
            var json = JsonSerializer.Serialize(testObject);

            var handler = new FakeHttpMessageHandler(req => Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }));

            var client = new HttpClient(handler);
            var caller = new LeyLineCaller(null, client);

            var result = await caller.GetAsync<dynamic>("https://fake.url");

            JsonSerializer.Serialize(result).Should().Contain("Hello");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnFileResponse()
        {
            var content = new MemoryStream(Encoding.UTF8.GetBytes("file content"));
            var responseContent = new StreamContent(content);
            responseContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            responseContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "test.txt"
            };

            var handler = new FakeHttpMessageHandler(req => Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent
            }));

            var client = new HttpClient(handler);
            var caller = new LeyLineCaller(null, client);

            var result = await caller.GetAsync<EchoShard>("https://fake.url");

            result.Should().BeOfType<EchoShard>();
            var file = result as EchoShard;
            file.FileName.Should().Be("test.txt");
            file.ContentType.Should().Be("application/octet-stream");
            new StreamReader(file.Content).ReadToEnd().Should().Be("file content");
        }

        [Fact]
        public async Task PostAsync_ShouldSendJsonPayload()
        {
            string capturedPayload = null;

            var handler = new FakeHttpMessageHandler(async req =>
            {
                capturedPayload = await req.Content.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            });

            var client = new HttpClient(handler);
            var caller = new LeyLineCaller(null, client);

            var payload = new { Id = 1, Name = "Test" };
            await caller.PostAsync<dynamic>("https://fake.url", payload);

            capturedPayload.Should().Contain("\"Id\":1");
            capturedPayload.Should().Contain("\"Name\":\"Test\"");
        }

        [Fact]
        public void Get_ShouldRunSynchronously()
        {
            var handler = new FakeHttpMessageHandler(req => Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Message\":\"sync\"}", Encoding.UTF8, "application/json")
            }));

            var client = new HttpClient(handler);
            var caller = new LeyLineCaller(null, client);

            var result = caller.Get<dynamic>("https://fake.url");

            JsonSerializer.Serialize(result).Should().Contain("sync");
        }

        [Fact]
        public void Post_ShouldRunSynchronously()
        {
            var handler = new FakeHttpMessageHandler(req => Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Success\":true}", Encoding.UTF8, "application/json")
            }));

            var client = new HttpClient(handler);
            var caller = new LeyLineCaller(null, client);

            var result = caller.Post<dynamic>("https://fake.url", new { Action = "test" });

            JsonSerializer.Serialize(result).Should().Contain("Success");
        }
    }
}
