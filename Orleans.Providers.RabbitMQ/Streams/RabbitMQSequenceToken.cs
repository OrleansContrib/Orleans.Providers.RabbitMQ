using Orleans.Streams;

namespace Orleans.Providers.RabbitMQ.Streams
{
    public class RabbitMQSequenceToken : StreamSequenceToken
    {
        public ulong DeliveryTag { get; set; }

        public RabbitMQSequenceToken(ulong deliveryTag)
        {
            DeliveryTag = deliveryTag;
        }

        public override int CompareTo(StreamSequenceToken other)
        {
            var o = (RabbitMQSequenceToken)other;
            return DeliveryTag > o.DeliveryTag ? 1 : (DeliveryTag < o.DeliveryTag ? -1 : 0);
        }

        public override bool Equals(StreamSequenceToken other)
        {
            return DeliveryTag == ((RabbitMQSequenceToken)other).DeliveryTag;
        }
    }
}
