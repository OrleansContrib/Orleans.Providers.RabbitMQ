using System.Threading.Tasks;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Interfaces
{
    public interface IProducerGrain : IGrainWithGuidKey
    {
        Task Simulate();
        Task Tick();
    }
}
