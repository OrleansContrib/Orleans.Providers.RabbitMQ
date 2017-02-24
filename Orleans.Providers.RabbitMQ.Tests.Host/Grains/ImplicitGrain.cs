using System.Threading.Tasks;
using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using System;
using Orleans.Streams;

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

        public async Task OnCompletedAsync()
        {

        }

        public async Task OnErrorAsync(Exception ex)
        {

        }

        public async Task OnNextAsync(string item, StreamSequenceToken token = null)
        {

        }
    }
}
