using System.Threading.Tasks;

namespace Orleans.Providers.RabbitMQ.Tests.Host.Interfaces
{
    public interface IConsumerGrain : IGrainWithGuidKey
    {
        Task Activate();
    }
}
