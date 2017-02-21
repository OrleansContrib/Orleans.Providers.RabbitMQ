using System;
using System.Collections.Generic;
using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Streams
{
    internal class RabbitMQBatchContainer : IBatchContainer
    {
        private byte[] body;

        public RabbitMQBatchContainer(byte[] body)
        {
            this.body = body;
        }

        public StreamSequenceToken SequenceToken { get; set; }

        public Guid StreamGuid { get; set; }

        public string StreamNamespace { get; set; }

        public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
        {
            return new List<Tuple<T, StreamSequenceToken>> { new Tuple<T, StreamSequenceToken>(default(T), null) };
        }

        public bool ImportRequestContext()
        {
            return false;
        }

        public bool ShouldDeliver(IStreamIdentity stream, object filterData, StreamFilterPredicate shouldReceiveFunc)
        {
            return false;
        }
    }
}