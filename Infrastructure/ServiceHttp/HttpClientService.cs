using System.Text;

namespace Infrastructure.ServiceHttp
{
    public class HttpClientService<Tin, TOut>(HttpClient httpClient) : IHttpClientService<Tin, TOut>
    {
        public async Task<TOut> Post(string url, Tin data, string idempotencyKey)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("X-Idempotency-Key", idempotencyKey);
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Mercado Pago devolvió el estado {(int)response.StatusCode}: {content}");

            return System.Text.Json.JsonSerializer.Deserialize<TOut>(content)
                ?? throw new InvalidOperationException("Mercado Pago no devolvió un pago válido.");
        }
    }
}
