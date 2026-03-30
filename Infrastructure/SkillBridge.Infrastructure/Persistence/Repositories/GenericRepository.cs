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

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> expression, bool asNoTracking = false)
    {
        var query = _dbSet.Where(expression);
        return asNoTracking ? query.AsNoTracking() : query;
    }

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);
    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        => await _dbSet.AnyAsync(expression);
}

