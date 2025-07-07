using FashionShopSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FashionShopSystem.Infrastructure.Repositories
{
	public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
	{
		protected readonly FashionShopContext _context;
		protected readonly DbSet<TEntity> _dbSet;

		public RepositoryBase(FashionShopContext context)
		{
			_context = context;
			_dbSet = _context.Set<TEntity>();
		}

		public virtual async Task<List<TEntity>> GetAllAsync()
			=> await _dbSet.AsQueryable().ToListAsync();

		public virtual async Task<TEntity?> GetByIdAsync(object id)
			=> await _dbSet.FindAsync(id);

		public virtual async Task AddAsync(TEntity entity)
		{
			_dbSet.Add(entity);
			await _context.SaveChangesAsync();
		}

		public virtual async Task UpdateAsync(TEntity entity)
		{
			_dbSet.Update(entity);
			await _context.SaveChangesAsync();
		}

		public virtual async Task DeleteAsync(TEntity entity)
		{
			_dbSet.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}
