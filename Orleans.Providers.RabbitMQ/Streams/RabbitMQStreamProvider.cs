using Orleans.Providers.Streams.Common;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQStreamProvider : PersistentStreamProvider<RabbitMQAdapterFactory>
    {
    }

    public abstract class RabbitMQStreamProvider<TCustomMapper> : PersistentStreamProvider<RabbitMQAdapterFactory<TCustomMapper>> where TCustomMapper : IRabbitMQCustomMapper
    {
    }
}
