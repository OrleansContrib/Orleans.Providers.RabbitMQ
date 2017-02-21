using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapter : IQueueAdapter
    {
        private RabbitMQStreamProviderConfig _config;
        private IConnection _connection;
        private IModel _model;

        public StreamProviderDirection Direction { get; private set; }

        public bool IsRewindable { get; private set; }

        public string Name { get; private set; }
        
        public RabbitMQAdapter(RabbitMQStreamProviderConfig config)
        {
            _config = config;
        }

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return RabbitMQAdapterReceiver.Create(_config);
        }

        public async Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            if (_connection == null)
                await Task.Run(async() => await CreateConnection());
            await Task.Run(() => {
                foreach (var e in events)
                {
                    var bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(e));
                    _model.BasicPublish(_config.Exchange, _config.RoutingKey, null, bytes);
                }
            });
        }

        private Task CreateConnection()
        {
            var factory = new ConnectionFactory();
            factory.HostName = _config.HostName;
            factory.VirtualHost = _config.VirtualHost;
            factory.UserName = _config.Username;
            factory.Password = _config.Password;
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_config.Exchange, ExchangeType.Direct, false, false, null);
            _model.QueueDeclare(_config.Queue, false, false, false, null);
            _model.QueueBind(_config.Queue, _config.Exchange, _config.RoutingKey, null);
            return TaskDone.Done;
        }
    }
}
