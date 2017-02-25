using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;
using RabbitMQ.Client;
using Orleans.Runtime;
using Orleans.Providers.Streams.Common;
using Newtonsoft.Json;
using System.Text;
using System.Reflection;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapterReceiver : IQueueAdapterReceiver
    {
        private RabbitMQStreamProviderConfig _config;
        private IConnection _connection;
        private IRabbitMQCustomMapper _customMapper;
        private Logger _logger;
        private IModel _model;
        private string _providerName;

        public static IQueueAdapterReceiver Create(RabbitMQStreamProviderConfig config, Logger logger, string providerName, IRabbitMQCustomMapper customMapper)
        {
            return new RabbitMQAdapterReceiver(config, logger, providerName, customMapper);
        }

        public RabbitMQAdapterReceiver(RabbitMQStreamProviderConfig config, Logger logger, string providerName, IRabbitMQCustomMapper customMapper)
        {
            _config = config;
            _customMapper = customMapper;
            _logger = logger;
            _providerName = providerName;
        }
        
        public async Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
        {
            var queueMessages = await Task.Run(async () =>
            {
                if (_connection == null)
                    await CreateConnection();
                var result = _model.BasicGet(_config.Queue, true);
                if (result == null)
                    return null;
                var container = new RabbitMQBatchContainer(result.Body, _customMapper);
                container.StreamGuid = Guid.Empty;
                container.StreamNamespace = _config.Namespace;
                container.SequenceToken = new EventSequenceToken((long)result.DeliveryTag);
                if (_customMapper != null)
                {
                    var streamMap = _customMapper.MapToStream(result.Body, _config.Namespace);
                    container.StreamGuid = streamMap.Item1;
                    container.StreamNamespace = streamMap.Item2;
                }
                return new List<IBatchContainer> { container };
            });
            if (queueMessages != null && _logger.SeverityLevel >= Severity.Verbose)
            {
                foreach (var message in queueMessages)
                {
                    _logger.Verbose($"Received message {((EventSequenceToken)message.SequenceToken).SequenceNumber}.");
                }
            }
            return queueMessages;
        }

        private async Task CreateConnection()
        {
            var factory = new ConnectionFactory();
            factory.HostName = _config.HostName;
            factory.VirtualHost = _config.VirtualHost;
            factory.UserName = _config.Username;
            factory.Password = _config.Password;
            _connection = factory.CreateConnection($"{_providerName}_Consumer");
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_config.Exchange, _config.ExchangeType, _config.ExchangeDurable, _config.AutoDelete, null);
            _model.QueueDeclare(_config.Queue, _config.QueueDurable, false, false, null);
            _model.QueueBind(_config.Queue, _config.Exchange, _config.RoutingKey, null);
        }

        public Task Initialize(TimeSpan timeout)
        {
            return TaskDone.Done;
        }

        public Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
        {
            // TODO: Ack messages, if required.
            return TaskDone.Done;
        }

        public Task Shutdown(TimeSpan timeout)
        {
            // TODO: Handle shutdown.
            throw new NotImplementedException();
        }
    }
}