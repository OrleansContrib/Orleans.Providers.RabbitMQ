using Orleans.Providers.RabbitMQ.Tests.Host.Interfaces;
using System;
using System.Threading.Tasks;

namespace Orleans.Providers.RabbitMQ.Test.Host.Bootstrap
{
    public class RabbitMQTestBootstrap : IBootstrapProvider
    {
        public string Name { get; set; }

        public Task Close()
        {
            return Task.CompletedTask;
        }

        public async Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            await providerRuntime.GrainFactory.GetGrain<IProducerGrain>(Guid.Empty).Simulate();
        }
    }
}
