using System;
using System.Linq;
using System.Linq.Expressions;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlazorSozluk.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbContext _dbContext;

    protected DbSet<TEntity> _entity => _dbContext.Set<TEntity>();

    public GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }


    #region Insert Methods

    public virtual int Add(TEntity entity)
    {
        _entity.Add(entity);
        return _dbContext.SaveChanges();
    }

    public virtual int Add(IEnumerable<TEntity> entities)
    {
        if (entities != null && !entities.Any())
            return 0;

        ArgumentNullException.ThrowIfNull(entities);
        _entity.AddRange(entities);

        return _dbContext.SaveChanges();
    }

    public virtual async Task<int> AddAsync(TEntity entity)
    {
        await _entity.AddAsync(entity);
        return _dbContext.SaveChanges();

    }

    public virtual async Task<int> AddAsync(IEnumerable<TEntity> entities)
    {
        if (entities != null && !entities.Any())
            return 0;

        ArgumentNullException.ThrowIfNull(entities);
        await _entity.AddRangeAsync(entities);
        return await _dbContext.SaveChangesAsync();

    }

    #endregion

    #region Add or Update Methods
    public int AddOrUpdate(TEntity entity)
    {
        if (!_entity.Local.Any(i => EqualityComparer<Guid>.Default.Equals(i.Id, entity.Id)))
            _dbContext.Update(entity);

        return _dbContext.SaveChanges();
    }

    public Task<int> AddOrUpdateAsync(TEntity entity)
    {
        if (!_entity.Local.Any(i => EqualityComparer<Guid>.Default.Equals(i.Id, entity.Id)))
            _dbContext.Update(entity);

        return _dbContext.SaveChangesAsync();
    }

    #endregion


    #region Bulk Methods
    public  virtual async Task BulkAdd(IEnumerable<TEntity> entities)
    {

        if (entities != null && !entities.Any())
            await Task.CompletedTask;

        ArgumentNullException.ThrowIfNull(entities);

        await _entity.AddRangeAsync(entities);
      

        await _dbContext.SaveChangesAsync();
    }

    public virtual Task BulkDelete(Expression<Func<TEntity, bool>> predicate)
    {
        IQueryable<TEntity> query = _entity;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        _dbContext.RemoveRange(query);

        return _dbContext.SaveChangesAsync();
    }

    public virtual Task BulkDelete(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        _dbContext.RemoveRange(entities);

        return _dbContext.SaveChangesAsync();

    }

    public virtual Task BulkDeleteById(IEnumerable<Guid> ids)
    {
        if (ids != null && !ids.Any())
            return Task.CompletedTask;

        _dbContext.RemoveRange(_entity.Where(i => ids.Contains(i.Id)));
        return _dbContext.SaveChangesAsync();
    }

    public virtual Task BulkUpdate(IEnumerable<TEntity> entities)
    {
        _entity.AttachRange(entities);

        foreach (var entity in entities)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        return _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Delete Methods
    public virtual int Delete(TEntity entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _entity.Attach(entity);
        }

        ArgumentNullException.ThrowIfNull(entity);

        _entity.Remove(entity);

        return _dbContext.SaveChanges();
    }

    public virtual int Delete(Guid id)
    {
        var entity = _entity.Find(id);

        ArgumentNullException.ThrowIfNull(entity);

        return Delete(entity);
    }

    public virtual Task<int> DeleteAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _entity.Attach(entity);
        }

        _entity.Remove(entity);

        return _dbContext.SaveChangesAsync();
    }

    public virtual Task<int> DeleteAsync(Guid id)
    {
        var entity = _entity.Find(id);

        ArgumentNullException.ThrowIfNull(entity);

        return DeleteAsync(entity);
    }

    public virtual bool DeleteRange(Expression<Func<TEntity, bool>> predicate)
    {
        _dbContext.RemoveRange(_entity.Where(predicate));

        return _dbContext.SaveChanges() > 0;
    }

    public virtual async Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
    {
        _dbContext.RemoveRange(_entity.Where(predicate));

        return await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Get Methods

    public virtual IQueryable<TEntity> AsQueryable() => _entity.AsQueryable();

    public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool noTrackin = true, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = _entity.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        query = ApplyIncludes(query, includes);

        if (noTrackin)
            query = query.AsNoTracking();

        return query;
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTrackin = true, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _entity;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        query = ApplyIncludes(query, includes);
        if (noTrackin)
            query = query.AsNoTracking();

        ArgumentNullException.ThrowIfNull(query);

        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<List<TEntity>> GetAll(bool noTracking = true)
    {
        IQueryable<TEntity> query = _entity;

        if (noTracking)
            query.AsNoTracking();

        ArgumentNullException.ThrowIfNull(query);

        return await query.ToListAsync();
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
    {

        TEntity found = await _entity.FindAsync(id);

        if (found == null)
            return null;

        if (noTracking)
            _dbContext.Entry(found).State = EntityState.Detached;

        ArgumentNullException.ThrowIfNull(includes);
        foreach (Expression<Func<TEntity, object>> include in includes)
        {
            _dbContext.Entry(found).Reference(include).Load();
        }

        return found;

    }

    public virtual async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _entity;

        if(predicate != null)
        {
            query = query.Where(predicate);
        }

        foreach(Expression<Func<TEntity,object>> include in includes)
        {
            query = query.Include(include);
        }

        if(orderBy != null)
        {
            query = orderBy(query);
        }

        if(noTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.ToListAsync();
    }

    public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool noTrackin = true, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _entity;

        if(predicate != null)
        {
            query = query.Where(predicate);
        }
        query = ApplyIncludes(query, includes);
        if (noTrackin)
            query = query.AsNoTracking();

        return await query.SingleOrDefaultAsync();

    }

    #endregion
    #region Update Methods
    public virtual int Update(TEntity entity)
    {
        _entity.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;

        return _dbContext.SaveChanges();
    }

    public virtual async Task<int> UpdateAsync(TEntity entity)
    {
        _entity.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;

        return await _dbContext.SaveChangesAsync();
    }
    #endregion


    #region SaveChanges Methods 
    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }

    #endregion

    private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query,params Expression<Func<TEntity, object>>[] includes)
    {
        if(includes != null)
        {
            foreach(var item in includes)
            {
                query = query.Include(item);
            }

        }
        return query;
    }
}

