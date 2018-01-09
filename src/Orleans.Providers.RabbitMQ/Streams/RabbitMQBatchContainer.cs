using Orleans.Streams;
using System;
using System.Collections.Generic;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQBatchContainer : IBatchContainer
    {
        public ulong Tag { get; private set; }
        private byte[] _body;
        private IRabbitMQMapper _mapper;

        public RabbitMQBatchContainer(ulong tag, byte[] body, IRabbitMQMapper mapper)
        {
            Tag = tag;
            _body = body;
            _mapper = mapper;
        }

        public StreamSequenceToken SequenceToken { get; set; }

        public Guid StreamGuid { get; set; }

        public string StreamNamespace { get; set; }

        public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
        {
            var message = _mapper.MapToType<T>(_body);
            if (message == null)
                return new List<Tuple<T, StreamSequenceToken>>();
            return new List<Tuple<T, StreamSequenceToken>> { new Tuple<T, StreamSequenceToken>(message, null) };
        }
        
        public bool ImportRequestContext()
        {
            return true;
        }

        public bool ShouldDeliver(IStreamIdentity stream, object filterData, StreamFilterPredicate shouldReceiveFunc)
        {
            return true;
        }
    }
}