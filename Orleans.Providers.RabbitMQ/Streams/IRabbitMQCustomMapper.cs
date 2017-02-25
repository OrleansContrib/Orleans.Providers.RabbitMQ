using Orleans.Runtime;
using System;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public interface IRabbitMQCustomMapper
    {
        void Init(Logger logger);
        T MapToType<T>(byte[] message);
        Tuple<Guid, string> MapToStream(byte[] message, string streamNamespace);
    }
}
