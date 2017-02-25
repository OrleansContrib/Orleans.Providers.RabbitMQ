using System;
using System.Collections.Generic;
using Orleans.Streams;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQBatchContainer : IBatchContainer
    {
        private byte[] _body;
        private IRabbitMQCustomMapper _customMapper;

        public RabbitMQBatchContainer(byte[] body, IRabbitMQCustomMapper customMapper)
        {
            _body = body;
            _customMapper = customMapper;
        }

        public StreamSequenceToken SequenceToken { get; set; }

        public Guid StreamGuid { get; set; }

        public string StreamNamespace { get; set; }

        public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
        {
            if (_customMapper != null)
            {
                var message = _customMapper.MapToType<T>(_body);
                if (message == null)
                    return new List<Tuple<T, StreamSequenceToken>>();
                return new List<Tuple<T, StreamSequenceToken>> { new Tuple<T, StreamSequenceToken>(message, null) };
            }
            if (_body is T)
                return new List<Tuple<T, StreamSequenceToken>> { new Tuple<T, StreamSequenceToken>((T)(object)_body, null) };
            T item = JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(_body));
            return new List<Tuple<T, StreamSequenceToken>> { new Tuple<T, StreamSequenceToken>(item, null) };
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