using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Streams;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapter : IQueueAdapter
    {
        private RabbitMQStreamProviderOptions _config;
        private IConnection _connection;
        private ILoggerFactory _loggerFactory;
        private IRabbitMQMapper _mapper;
        private IModel _model;
        private ConcurrentDictionary<QueueId, object> _queues;
        private IStreamQueueMapper _streamQueueMapper;

        public StreamProviderDirection Direction { get; private set; }

        public bool IsRewindable { get { return false; } }

        public string Name { get; private set; }

        public RabbitMQAdapter(RabbitMQStreamProviderOptions config, ILoggerFactory loggerFactory, string providerName, IStreamQueueMapper streamQueueMapper, IRabbitMQMapper mapper)
        {
            Direction = config.Mode;
            _config = config;
            _loggerFactory = loggerFactory;
            Name = providerName;
            _streamQueueMapper = streamQueueMapper;
            _mapper = mapper;
            _queues = new ConcurrentDictionary<QueueId, object>();
        }

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return RabbitMQAdapterReceiver.Create(_config, _loggerFactory, queueId, Name, _mapper);
        }

        public async Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            await Task.Run(() => QueueMessageBatchExternal<T>(streamGuid, streamNamespace, events, token, requestContext));
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
            lock (_model)
            {
                _model.ExchangeDeclare(_config.Exchange, _config.ExchangeType, _config.ExchangeDurable, _config.AutoDelete, null);
                _model.QueueDeclare(_config.Queue, _config.QueueDurable, false, false, null);
                _model.QueueBind(_config.Queue, _config.Exchange, _config.RoutingKey, null);
            }
        }
    }
}
