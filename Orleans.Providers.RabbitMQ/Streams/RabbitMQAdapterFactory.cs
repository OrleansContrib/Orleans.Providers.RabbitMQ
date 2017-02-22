using System;
using System.Threading.Tasks;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapterFactory : IQueueAdapterFactory
    {
        private RabbitMQStreamProviderConfig _config;
        private Logger _logger;

        protected Func<QueueId, Task<IStreamFailureHandler>> StreamFailureHandlerFactory { private get; set; }

        public void Init(IProviderConfiguration config, string providerName, Logger logger, IServiceProvider serviceProvider)
        {
            _config = new RabbitMQStreamProviderConfig(config);
            _logger = logger;
        }

        public Task<IQueueAdapter> CreateAdapter()
        {
            IQueueAdapter adapter = new RabbitMQAdapter(_config);
            return Task.FromResult(adapter);
        }

        public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
        {
            return StreamFailureHandlerFactory(queueId);
        }

        public IQueueAdapterCache GetQueueAdapterCache()
        {
            return new SimpleQueueAdapterCache(10, _logger);
        }

        public IStreamQueueMapper GetStreamQueueMapper()
        {
            return new HashRingBasedStreamQueueMapper(10, "Prefix");
        }
    }
}
