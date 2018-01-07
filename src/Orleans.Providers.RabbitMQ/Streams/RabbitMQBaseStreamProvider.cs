using Orleans.Providers.Streams.Common;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public abstract class RabbitMQBaseStreamProvider<TMapper> : PersistentStreamProvider<RabbitMQAdapterFactory<TMapper>> where TMapper : IRabbitMQMapper
    {
    }
}
