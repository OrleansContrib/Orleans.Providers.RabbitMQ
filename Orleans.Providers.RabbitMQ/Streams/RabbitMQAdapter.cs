using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using Orleans.Runtime;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapter : IQueueAdapter
    {
        private RabbitMQStreamProviderConfig _config;
        private IConnection _connection;
        private Logger _logger;
        private IModel _model;

        public StreamProviderDirection Direction { get { return StreamProviderDirection.ReadWrite; } }

        public bool IsRewindable { get { return false; } }

        public string Name { get; private set; }
        
        public RabbitMQAdapter(RabbitMQStreamProviderConfig config, Logger logger, string providerName)
        {
            _config = config;
            _logger = logger;
            Name = providerName;
        }

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return RabbitMQAdapterReceiver.Create(_config, _logger, Name);
        }

        public async Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            await Task.Run(() => {
                if (_connection == null)
                    CreateConnection();
                foreach (var e in events)
                {
                    var bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(e));
                    _model.BasicPublish(_config.Exchange, _config.RoutingKey, null, bytes);
                }
            });
        }

        private void CreateConnection()
        {
            var factory = new ConnectionFactory();
            factory.HostName = _config.HostName;
            factory.VirtualHost = _config.VirtualHost;
            factory.UserName = _config.Username;
            factory.Password = _config.Password;
            _connection = factory.CreateConnection($"{Name}_Producer");
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_config.Exchange, ExchangeType.Direct, false, false, null);
            _model.QueueDeclare(_config.Queue, false, false, false, null);
            _model.QueueBind(_config.Queue, _config.Exchange, _config.RoutingKey, null);
        }
    }
}
