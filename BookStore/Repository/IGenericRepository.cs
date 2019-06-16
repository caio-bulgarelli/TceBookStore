using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository
{
    public interface IGenericRepository<T, U>
    {
        Task<T> GetAsync(U id);

        IQueryable<T> Query();

        Task InsertAsync(T entity);

        Task<T> DeleteAsync(U id);

        Task UpdateAsync(T oldEntity, T entity);

        Task<bool> Exists(U id);
    }
}