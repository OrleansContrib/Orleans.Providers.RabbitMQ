using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Streams;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using Orleans.Runtime;
using System.Collections.Concurrent;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapter : IQueueAdapter
    {
        private RabbitMQStreamProviderConfig _config;
        private IConnection _connection;
        private Logger _logger;
        private IRabbitMQMapper _mapper;
        private IModel _model;
        private ConcurrentDictionary<QueueId, object> _queues;
        private IStreamQueueMapper _streamQueueMapper;

        public StreamProviderDirection Direction { get { return StreamProviderDirection.ReadWrite; } }

        public bool IsRewindable { get { return false; } }

        public string Name { get; private set; }
        
        public RabbitMQAdapter(RabbitMQStreamProviderConfig config, Logger logger, string providerName, IStreamQueueMapper streamQueueMapper, IRabbitMQMapper mapper)
        {
            _config = config;
            _logger = logger;
            Name = providerName;
            _streamQueueMapper = streamQueueMapper;
            _mapper = mapper;
            _queues = new ConcurrentDictionary<QueueId, object>();
        }

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return RabbitMQAdapterReceiver.Create(_config, _logger, queueId, Name, _mapper);
        }

        public async Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            await Task.Run(() => QueueMessageBatchExternal<T>(streamGuid, streamNamespace, events,  token, requestContext));
        }

        private void QueueMessageBatchExternal<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            if (_connection == null)
                CreateConnection();

            var queue = _streamQueueMapper.GetQueueForStream(streamGuid, streamNamespace);
            
            // TODO: Handle queues.

            foreach (var e in events)
            {
                var bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(e));
                _model.BasicPublish(_config.Exchange, _config.RoutingKey, null, bytes);
            }
        }

        private void CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.HostName,
                VirtualHost = _config.VirtualHost,
                UserName = _config.Username,
                Password = _config.Password
            };
            _connection = factory.CreateConnection($"{Name}_Producer");
            _model = _connection.CreateModel();
            _model.ExchangeDeclare(_config.Exchange, _config.ExchangeType, _config.ExchangeDurable, _config.AutoDelete, null);
            _model.QueueDeclare(_config.Queue, _config.QueueDurable, false, false, null);
            _model.QueueBind(_config.Queue, _config.Exchange, _config.RoutingKey, null);
        }
    }
}
