using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace hey_url_challenge_code_dotnet.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> AddAsync(T entity);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task UpdateAsync(T entity);
    }
}
