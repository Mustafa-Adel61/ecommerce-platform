namespace eCommerce.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name, ct);
        }
    }
}
