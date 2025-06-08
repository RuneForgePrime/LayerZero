using LayerZero.Tools.Configs.LeyLine;
using LayerZero.Tools.CoreClasses;
using System.Text;
using System.Text.Json;

namespace LayerZero.Tools.Managements
{
    public class LeyLineCaller
    {
        private HttpClient _httpClient;
        public LeyLineCaller(SigilBinder header, HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();

            if (header is { IsValid: true })
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(header.TokenType, header.AccessToken);
            }
        }


        public async Task<HttpResponseMessage> _GetAsync(string uri)
            => await this._httpClient.GetAsync(uri);

        public async Task<HttpResponseMessage> _PostAsync(string uri, object Object)
        {
            var param = JsonSerializer.Serialize(Object);
            var content = new StringContent(param, Encoding.UTF8, "application/json");
            return await this._httpClient.PostAsync(uri, content);
        }



        public HttpResponseMessage _Get(string uri)
            => GeasMaster.RunSync<HttpResponseMessage>(() => _GetAsync(uri));


        public HttpResponseMessage _Post(string uri, object Object)
            => GeasMaster.RunSync<HttpResponseMessage>(() => _PostAsync(uri, Object));


        private async Task<object> ParseResponse<T>(HttpResponseMessage message)
        {
            var contentType = message.Content.Headers.ContentType?.MediaType?.ToLowerInvariant() ?? "";
            var hasFileName = message.Content.Headers.ContentDisposition?.FileName is not null;

            return contentType switch
            {
                "application/json" => SygilParser.Parse<T>(await message.Content.ReadAsStringAsync()),

                _ when hasFileName || contentType == "application/octet-stream" => await CreateFileResult(message),

                _ when contentType.StartsWith("text/") => await message.Content.ReadAsStringAsync(),

                _ => await message.Content.ReadAsStringAsync()
            };
        }

        private static async Task<EchoShard> CreateFileResult(HttpResponseMessage message)
        {
            var fileName = message.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "downloaded_file.bin";
            var contentType = message.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

            var memoryStream = new MemoryStream();
            using (var responseStream = await message.Content.ReadAsStreamAsync())
            {
                await responseStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;

            return new EchoShard
            {
                FileName = fileName,
                ContentType = contentType,
                Content = memoryStream
            };
        }

        public async Task<object> GetAsync<T>(string uri)
            => await ParseResponse<T>(await _GetAsync(uri));

        public async Task<object> PostAsync<T>(string uri, object Object)
            => await ParseResponse<T>(await _PostAsync(uri, Object));

        public object Get<T>(string uri)
            => GeasMaster.RunSync<object>(() => GetAsync<T>(uri));

        public object Post<T>(string uri, object Object)
            => GeasMaster.RunSync<object>(() => PostAsync<T>(uri, Object));

    }
}
