using OKX.Api.Public;

namespace STApp.Client.Service
{
    public interface IOKXSubscriber
    {
        void Dispose();
        IEnumerable<OkxPublicTicker> GetTickers();
        Task SubscribeTicker(string instrumentId, Action<OkxPublicTicker> tickCallback);
        void Unsubscribe(string instId);
    }
}