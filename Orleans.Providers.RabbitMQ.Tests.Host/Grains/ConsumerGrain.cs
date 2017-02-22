using System;
using System.Threading.Tasks;
using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Grains
{
    public class ConsumerGrain : Grain, IConsumerGrain, IAsyncObserver<string>
    {
        public async override Task OnActivateAsync()
        {
            var stream = GetStreamProvider("Default").GetStream<string>(this.GetPrimaryKey(), "TestNamespace");
            await stream.SubscribeAsync(this);
        }

        public Task Activate()
        {
            return TaskDone.Done;
        }
        
        public Task OnCompletedAsync()
        {
            return TaskDone.Done;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return TaskDone.Done;
        }

        public Task OnNextAsync(string item, StreamSequenceToken token = null)
        {
            Console.WriteLine($"Got message '{item}'!");
            return TaskDone.Done;
        }
    }
}
