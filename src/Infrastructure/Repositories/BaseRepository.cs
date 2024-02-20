using Domain.Entities;
using Domain.Interfaces.Persistence;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationContext _context;
    protected DbSet<T> _dataSet;
    public BaseRepository(ApplicationContext context)
    {
        _context = context;
        _dataSet = context.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        _dataSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);

        if (entity == null)
            return false;

        _dataSet.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Exist(int id)
    {
        var entity = _dataSet.FirstOrDefault(e => e.Id == id);

        return entity != null;

    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dataSet.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeString = null, bool disableTracking = true)
    {
        IQueryable<T> query = _dataSet;
        if (disableTracking) query = query.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

        if (predicate != null) query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();
        return await query.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dataSet.SingleOrDefaultAsync(_ => _.Id.Equals(id));
    }

    public async Task<T> UpdateAsync(T entity)
    {
        var dbEntity = await GetByIdAsync(entity.Id);

        if (dbEntity == null)
            return null;

        _context.Entry(dbEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();

        return entity;

    }
}
