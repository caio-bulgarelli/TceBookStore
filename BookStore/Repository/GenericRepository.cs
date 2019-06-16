using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repository
{
    public abstract class GenericRepository<T, U> : IGenericRepository<T, U>
        where T : class, new()
    {
        protected ApplicationDbContext _dbContext { get; set; }

        public async Task<T> GetAsync(U id)
        {
            return await _dbContext.FindAsync<T>(id);
        }

        public IQueryable<T> Query()
        {
            return _dbContext.Set<T>().AsQueryable();
        }

        public async Task InsertAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> DeleteAsync(U id)
        {
            T entity = await _dbContext.FindAsync<T>(id);
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T oldEntity, T entity)
        {
            _dbContext.Entry(oldEntity).State = EntityState.Detached;
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> Exists(U id)
        {
            return (await _dbContext.FindAsync<T>(id)) != null;
        }
    }
}