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
            var guid = new Guid("66a35781-c48d-4404-938f-54211c5f49da");
            await providerRuntime.GrainFactory.GetGrain<IProducerGrain>(guid).Simulate();
            
            var guid2 = new Guid("c25fec47-99d2-48db-a774-471bf23c4cbf");
            await providerRuntime.GrainFactory.GetGrain<IProducerGrain>(guid2).Simulate();
        }
    }
}
