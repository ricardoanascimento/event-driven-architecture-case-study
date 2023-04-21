using SharedKernel.Models;

namespace SharedKernel.Services
{
    public interface IEventConsumer<T>
    {
        void StartConsuming(Action<T> onMessageReceived);
    }
}