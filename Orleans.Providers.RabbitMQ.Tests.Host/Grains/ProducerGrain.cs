using System;
using System.Threading.Tasks;
using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using Orleans.Streams;
using Orleans.Runtime;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Grains
{
    public class ProducerGrain : Grain, IProducerGrain
    {
        private int _counter;
        private IAsyncStream<string> _stream;

        public override Task OnActivateAsync()
        {
            var provider = GetStreamProvider("Default");
            _stream = provider.GetStream<string>(this.GetPrimaryKey(), "TestNamespace");
            return TaskDone.Done;
        }

        public Task Simulate()
        {
            RegisterTimer(OnSimulationTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            return TaskDone.Done;
        }

        public async Task Tick()
        {
            await OnSimulationTick(null);
        }

        private async Task OnSimulationTick(object state)
        {
            await SendMessages(_counter++.ToString());
            await SendMessages(_counter++.ToString(), _counter++.ToString());
        }

        private async Task SendMessages(params string[] messages)
        {
            GetLogger().Info("Sending message{0} '{1}'...",
                messages.Length > 1 ? "s" : "", string.Join(",", messages));
            
            if (messages.Length == 1)
            { 
                await _stream.OnNextAsync(messages[0]);
                return;
            }
            await _stream.OnNextBatchAsync(messages);
        }
    }
}
