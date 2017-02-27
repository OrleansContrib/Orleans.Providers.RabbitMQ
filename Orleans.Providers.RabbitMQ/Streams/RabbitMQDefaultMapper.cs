using System;
using Orleans.Runtime;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQDefaultMapper : IRabbitMQMapper
    {
        private Logger _logger;

        public void Init(Logger logger)
        {
            _logger = logger;
        }

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
