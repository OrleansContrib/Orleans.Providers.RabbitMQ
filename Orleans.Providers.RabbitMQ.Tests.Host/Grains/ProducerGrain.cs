using System;
using System.Threading.Tasks;
using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using Orleans.Streams;
using System.Text;
using System.Collections.Generic;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Grains
{
    public class ProducerGrain : Grain, IProducerGrain
    {
        private IAsyncStream<string> _stream;

        public async override Task OnActivateAsync()
        {
            var streamProvider = base.GetStreamProvider("Default");
            _stream = streamProvider.GetStream<string>(this.GetPrimaryKey(), "TestStreamNamespace");
        }

        public Task Simulate()
        {
            RegisterTimer(OnSimulationTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
            return TaskDone.Done;
        }

        private async Task OnSimulationTick(object state)
        {
            await _stream.OnNextAsync("Lipsum");
            await _stream.OnNextBatchAsync(new List<string>{ "Foo", "Bar" });
        }
    }
}
