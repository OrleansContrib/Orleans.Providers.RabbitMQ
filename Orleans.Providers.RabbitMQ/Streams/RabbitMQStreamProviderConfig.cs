using System;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQStreamProviderConfig
    {
        public string HostName { get; private set; }
        public int Port { get; private set; }
        public string VirtualHost { get; private set; }
        public string Exchange { get; private set; }
        public string Queue { get; private set; }
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
            Queue = config.Properties["Queue"];
            Namespace = config.Properties["Namespace"];
            RoutingKey = config.Properties["RoutingKey"];
            Username = config.Properties["Username"];
            Password = config.Properties["Password"];
        }
    }
}
