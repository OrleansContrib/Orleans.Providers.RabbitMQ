using System.Threading.Tasks;
using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using System;
using Orleans.Streams;
using Orleans.Runtime;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Grains
{
    [ImplicitStreamSubscription("TestNamespace")]
    public class ImplicitGrain : Grain, IImplicitGrain, IAsyncObserver<string>
    {
        private StreamSubscriptionHandle<string> _subscription;

        public async override Task OnActivateAsync()
        {
            var provider = GetStreamProvider("Default");
            var stream = provider.GetStream<string>(this.GetPrimaryKey(), "TestNamespace");
            _subscription = await stream.SubscribeAsync(this);
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task OnNextAsync(string item, StreamSequenceToken token = null)
        {
            GetLogger().Info("Received message '{0}'!", item);
            return Task.CompletedTask;
        }
    }
}
