using System;
using System.Threading.Tasks;
using Orleans.Runtime;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQAdapterFactory : IQueueAdapterFactory
    {
        private RabbitMQStreamProviderConfig _config;

        public Task<IQueueAdapter> CreateAdapter()
        {
            IQueueAdapter adapter = new RabbitMQAdapter(_config);
            return Task.FromResult(adapter);
        }

        public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
        {
            throw new NotImplementedException();
        }

        public IQueueAdapterCache GetQueueAdapterCache()
        {
            throw new NotImplementedException();
        }

        public IStreamQueueMapper GetStreamQueueMapper()
        {
            throw new NotImplementedException();
        }

        public void Init(IProviderConfiguration config, string providerName, Logger logger, IServiceProvider serviceProvider)
        {
            _config = new RabbitMQStreamProviderConfig(config);
        }
    }
}
