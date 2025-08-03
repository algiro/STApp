using ApiSharp.Models;
using ApiSharp.WebSocket;
using OKX.Api;
using OKX.Api.Public;
using System.Collections.Concurrent;

namespace STApp.Client.Service
{
    public class OKXSubscriber : IDisposable, IOKXSubscriber
    {
        private readonly OkxWebSocketApiClient ws = new OkxWebSocketApiClient();
        private ConcurrentDictionary<string, OkxPublicTicker> Tickers = new ConcurrentDictionary<string, OkxPublicTicker>();
        private ConcurrentDictionary<string, CallResult<WebSocketUpdateSubscription>> Subscriptions = new ConcurrentDictionary<string, CallResult<WebSocketUpdateSubscription>>();
        public async Task SubscribeTicker(string instrumentId, Action<OkxPublicTicker> tickCallback)
        {
            Console.WriteLine("Subscribing to ticker for instrument: " + instrumentId);

            var subscription = await ws.Public.SubscribeToTickersAsync((data) =>
            {
                Tickers.AddOrUpdate(data.InstrumentId, data, (key, oldValue) => data);
                tickCallback(data);
            }, instrumentId);
            Subscriptions.AddOrUpdate(instrumentId, subscription, (key, oldValue) => subscription);
        }

        public IEnumerable<OkxPublicTicker> GetTickers() => Tickers.Values;

        public void Dispose()
        {
            Console.WriteLine("Disposing OKXSubscriber and unsubscribing from all tickers.");
            foreach (var instId in Subscriptions.Keys)
            {
                Unsubscribe(instId);
            }
            ws.Dispose();
        }

        public void Unsubscribe(string instId)
        {
            Console.WriteLine("Unsubscribing from ticker for instrument: " + instId);
            if (!Subscriptions.TryRemove(instId, out var subscription))
            {
                Console.WriteLine($"No subscription found for {instId}");
                return;
            }
            _ = ws.UnsubscribeAsync(subscription.Data);
            Tickers.TryRemove(instId, out _);
        }
    }
}
