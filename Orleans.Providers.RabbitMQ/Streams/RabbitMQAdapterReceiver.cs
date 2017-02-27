using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;
using RabbitMQ.Client;
using Orleans.Runtime;
using Orleans.Providers.Streams.Common;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapterReceiver : IQueueAdapterReceiver
    {
        private RabbitMQStreamProviderConfig _config;
        private IConnection _connection;
        private Logger _logger;
        private IRabbitMQMapper _mapper;
        private IModel _model;
        private QueueId _queueId;
        private string _providerName;

        public static IQueueAdapterReceiver Create(RabbitMQStreamProviderConfig config, Logger logger, QueueId queueId, string providerName, IRabbitMQMapper mapper)
        {
            return new RabbitMQAdapterReceiver(config, logger, queueId, providerName, mapper);
        }

        public RabbitMQAdapterReceiver(RabbitMQStreamProviderConfig config, Logger logger, QueueId queueId, string providerName, IRabbitMQMapper mapper)
        {
            _config = config;
            _logger = logger;
            _queueId = queueId;
            _mapper = mapper;
            _providerName = providerName;
        }
        
        public async Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
        {
            return await Task.Run(() => GetQueueMessagesExternal(maxCount));
        }

        private IList<IBatchContainer> GetQueueMessagesExternal(int maxCount)
        {
            List<IBatchContainer> batches = null;
            int count = 0;
            
            while (true)
            {
                if (count == maxCount)
                    return batches;

                if (!IsConnected())
                    Connect();

                var result = _model.BasicGet(_config.Queue, true);

                if (result == null)
                    return batches;

                if (batches == null)
                    batches = new List<IBatchContainer>();

                batches.Add(CreateContainer(result));

                count++;
            }
        }

        private RabbitMQBatchContainer CreateContainer(BasicGetResult result)
        {
            var streamMap = _mapper.MapToStream(result.Body, _config.Namespace);
            var container = new RabbitMQBatchContainer(result.Body, _mapper)
            {
                StreamGuid = streamMap.Item1,
                StreamNamespace = streamMap.Item2,
                SequenceToken = new EventSequenceToken((long)result.DeliveryTag)
            };

            return container;
        }

        private bool IsConnected()
        {
            return _connection != null && _connection.IsOpen;
        }

        private void Connect()
        {
            var partitionName = _mapper.GetPartitionName(_config.Queue, _queueId);
            var factory = new ConnectionFactory()
            {
                HostName = _config.HostName,
                VirtualHost = _config.VirtualHost,
                UserName = _config.Username,
                Password = _config.Password
            };
            _connection = factory.CreateConnection($"{_providerName}_Consumer");
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_config.Exchange, _config.ExchangeType, _config.ExchangeDurable, _config.AutoDelete, null);
            _model.QueueDeclare(partitionName, _config.QueueDurable, false, false, null);
            foreach (var partitionKey in _mapper.GetPartitionKeys(_queueId, _config.NumQueues))
            {
                _model.QueueBind(partitionName, _config.Exchange, partitionKey, null);
            }
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