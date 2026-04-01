using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Interfaces;
using SkillBridge.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly SkillBridgeDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(SkillBridgeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);
    
    public IQueryable<T> GetAllQueryable()
        => _dbSet.AsQueryable();

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> expression, bool asNoTracking = false)
    {
        var query = _dbSet.Where(expression);
        return asNoTracking ? query.AsNoTracking() : query;
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(expression, cancellationToken);
}

