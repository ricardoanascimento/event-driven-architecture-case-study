using SharedKernel.Models;

namespace SharedKernel.Services
{
    public interface IDataHandler<T>
    {
        Task InsertAsync(T document);
    }
}