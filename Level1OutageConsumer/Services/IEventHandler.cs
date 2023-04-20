public interface IEventHandler
{
    void StartConsuming(Action<TurbineData> onMessageReceived);
}