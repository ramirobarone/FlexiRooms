
namespace Infrastructure.ServiceHttp
{
    public interface IHttpClientService<Tin, TOut>
    {
        Task<TOut> Post(string url, Tin data, string idempotencyKey);
    }
}