using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces.Persistence;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                    string includeString = null,
                                    bool disableTracking = true);
    Task<T> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);

    Task<bool> Exist(int id);
}