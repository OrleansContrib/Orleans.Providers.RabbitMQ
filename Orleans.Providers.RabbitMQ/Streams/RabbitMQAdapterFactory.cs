using System;
using System.Threading.Tasks;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapterFactory<TMapper> : IQueueAdapterFactory where TMapper : IRabbitMQMapper
    {
        public const int NumQueuesDefaultValue = 8;

        private SimpleQueueAdapterCache _adapterCache;
        private int _cacheSize;
        private RabbitMQStreamProviderConfig _config;
        private IRabbitMQMapper _mapper;
        private Logger _logger;
        private string _providerName;
        private IStreamQueueMapper _streamQueueMapper;

        public RabbitMQAdapterFactory()
        {
            _mapper = Activator.CreateInstance<TMapper>();
        }

        protected Func<QueueId, Task<IStreamFailureHandler>> StreamFailureHandlerFactory { private get; set; }

        public void Init(IProviderConfiguration config, string providerName, Logger logger, IServiceProvider serviceProvider)
        {
            _config = new RabbitMQStreamProviderConfig(config);
            _providerName = providerName;
            _logger = logger;

            _mapper.Init(logger);

            _cacheSize = SimpleQueueAdapterCache.ParseSize(config, 4096);
            _adapterCache = new SimpleQueueAdapterCache(_cacheSize, logger);

            _streamQueueMapper = new HashRingBasedStreamQueueMapper(NumQueuesDefaultValue, _providerName);

            if (StreamFailureHandlerFactory == null)
            {
                StreamFailureHandlerFactory =
                    qid => Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler(false));
            }
        }

        public Task<IQueueAdapter> CreateAdapter()
        {
            IQueueAdapter adapter = new RabbitMQAdapter(_config, _logger, _providerName, _streamQueueMapper, _mapper);
            return Task.FromResult(adapter);
        }

        public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
        {
            return StreamFailureHandlerFactory(queueId);
        }

        public IQueueAdapterCache GetQueueAdapterCache()
        {
            return _adapterCache;
        }

        public IStreamQueueMapper GetStreamQueueMapper()
        {
            return _streamQueueMapper;
        }
    }
}
