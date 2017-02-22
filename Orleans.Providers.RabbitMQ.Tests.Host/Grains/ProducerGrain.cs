using System;
using System.Threading.Tasks;
using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using Orleans.Streams;
using System.Collections.Generic;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Grains
{
    public class ProducerGrain : Grain, IProducerGrain, IAsyncObserver<string>
    {
        private IAsyncStream<string> _stream;
        private IAsyncStream<string> _stream2;

        public async override Task OnActivateAsync()
        {
            var streamProvider = base.GetStreamProvider("Default");
            _stream = streamProvider.GetStream<string>(this.GetPrimaryKey(), "TestStreamNamespace");
            _stream2 = streamProvider.GetStream<string>(this.GetPrimaryKey(), "TestStreamNamespace2");
            await _stream.SubscribeAsync(this);
        }

        public Task OnCompletedAsync()
        {
            Console.WriteLine("OnCompletedAsync()");
            return TaskDone.Done;
        }

        public Task OnErrorAsync(Exception ex)
        {
            Console.WriteLine("OnErrorAsync("+ex+")");
            return TaskDone.Done;
        }

        public Task OnNextAsync(string item, StreamSequenceToken token = null)
        {
            Console.WriteLine("OnCompletedAsync("+item+","+token+")");
            return TaskDone.Done;
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
            await _stream2.OnNextAsync("Lipsum");
            await _stream2.OnNextBatchAsync(new List<string> { "Foo", "Bar" });
        }
    }
}
