namespace FashionShopSystem.Infrastructure.Repositories
{
	public interface IRepositoryBase<TEntity> where TEntity : class
	{
		Task<List<TEntity>> GetAllAsync();
		Task<TEntity?> GetByIdAsync(object id);
		Task AddAsync(TEntity entity);
		Task UpdateAsync(TEntity entity);
		Task DeleteAsync(TEntity entity);
	}
}
