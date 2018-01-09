using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQStreamProviderOptions
    {
        public const string SECTION_NAME = "Orleans.Providers.RabbitMQ";
        [JsonConverter(typeof(StringEnumConverter))]
        public StreamProviderDirection Mode { get; set; } = StreamProviderDirection.ReadWrite;
        public int NumberOfQueues { get; set; } = 8;
        public string HostName { get; set; }
        public int Port { get; set; } = 5671;
        public string VirtualHost { get; set; }
        public string Exchange { get; set; }
        public string ExchangeType { get; set; } = "direct";
        public bool ExchangeDurable { get; set; }
        public bool AutoDelete { get; set; }
        public string Queue { get; set; }
        public bool QueueDurable { get; set; }
        public string Namespace { get; set; }
        public string RoutingKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
