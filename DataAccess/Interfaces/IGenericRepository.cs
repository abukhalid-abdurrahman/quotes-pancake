using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quotes.DataAccess.Interfaces
{
    public interface IGenericRepository<in T, TK>
    {
        Task<TK> GetByIdAsync(int id);
        Task<IReadOnlyList<TK>> GetAllAsync();
        Task<TK> AddAsync(T entity);
        Task<TK> UpdateAsync(T entity);
        Task<TK> RemoveAsync(int id);
    }
}