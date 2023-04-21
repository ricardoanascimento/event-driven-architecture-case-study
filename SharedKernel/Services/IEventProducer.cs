public interface IEventProducer<T>
{
    void PublishEvent(T message);
}