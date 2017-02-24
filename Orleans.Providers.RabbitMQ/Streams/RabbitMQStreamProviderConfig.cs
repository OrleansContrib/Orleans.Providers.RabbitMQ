using RabbitMQ.Client;
using System;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQStreamProviderConfig
    {
        public string HostName { get; private set; }
        public int Port { get; private set; }
        public string VirtualHost { get; private set; }
        public string Exchange { get; private set; }
        public string ExchangeType { get; private set; }
        public bool ExchangeDurable { get; private set; }
        public bool AutoDelete { get; private set; }
        public string Queue { get; private set; }
        public bool QueueDurable { get; private set; }
        public string Namespace { get; private set; }
        public string RoutingKey { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public RabbitMQStreamProviderConfig(IProviderConfiguration config)
        {
            HostName = config.Properties["HostName"];
            Port = config.GetIntProperty("Port", 5671);
            VirtualHost = config.Properties["VirtualHost"];
            Exchange = config.Properties["Exchange"];
            ExchangeType = config.GetProperty("ExchangeType", "Direct").ToLowerInvariant();
            ExchangeDurable = config.GetBoolProperty("ExchangeDurable", false);
            AutoDelete = config.GetBoolProperty("AutoDelete", false);
            Queue = config.Properties["Queue"];
            QueueDurable = config.GetBoolProperty("QueueDurable", false);
            Namespace = config.Properties["Namespace"];
            RoutingKey = config.Properties["RoutingKey"];
            Username = config.Properties["Username"];
            Password = config.Properties["Password"];
        }
    }
}
