using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Grains
{
    public class ProducerGrain : Grain, IProducerGrain
    {
        private IAsyncStream<string> _stream;

        public async override Task OnActivateAsync()
        {
            var provider = GetStreamProvider("Default");
            _stream = provider.GetStream<string>(this.GetPrimaryKey(), "TestNamespace");
        }

        public Task Simulate()
        {
            RegisterTimer(OnSimulationTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            return TaskDone.Done;
        }

        private async Task OnSimulationTick(object state)
        {
            await _stream.OnNextAsync("Lipsum");
            await _stream.OnNextBatchAsync(new List<string> { "Foo", "Bar" });
        }
    }
}
