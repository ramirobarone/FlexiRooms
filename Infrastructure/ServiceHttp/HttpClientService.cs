using System.Text;

namespace Infrastructure.ServiceHttp
{
    public class HttpClientService<Tin, TOut>(HttpClient httpClient) : IHttpClientService<Tin, TOut>
    {
        public async Task<TOut> Post(string url, Tin data)
        {
            TOut? resultObject = default;
            httpClient.DefaultRequestHeaders.Add("X-Idempotency-Key", "");

            var conten = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, conten);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await conten.ReadAsStringAsync();

                if (conten is not null)
                {
                    resultObject = System.Text.Json.JsonSerializer.Deserialize<TOut>(content);

                    if (resultObject is TOut)
                        return (TOut)resultObject;
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception("Problemas al realizar el pago.");
            }

            return resultObject ?? throw new ArgumentNullException();
        }
    }
}
