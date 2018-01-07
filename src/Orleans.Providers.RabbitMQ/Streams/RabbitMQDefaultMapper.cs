using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQDefaultMapper : IRabbitMQMapper
    {
        private ILogger _logger;

        public RabbitMQDefaultMapper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(nameof(RabbitMQDefaultMapper));
        }

        public void Init() { }

        public Tuple<Guid, string> MapToStream(byte[] message, string streamNamespace)
        {
            return new Tuple<Guid, string>(Guid.Empty, streamNamespace);
        }

        public T MapToType<T>(byte[] message)
        {
            if (message is T)
                return (T)(object)message;
            T item = JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(message));
            return item;
        }

        public IEnumerable<string> GetPartitionKeys(QueueId queueId, int numQueues)
        {
            return new string[] { "#" };
        }

        public string GetPartitionName(string queue, QueueId queueId)
        {
            return queue;
        }
    }
}
