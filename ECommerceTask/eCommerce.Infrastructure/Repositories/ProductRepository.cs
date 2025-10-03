namespace eCommerce.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken ct = default)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IEnumerable<Product>> ListWithCategoryAsync(CancellationToken ct = default)
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync(ct);
        }


        public async Task<Product?> GetByCodeAsync(string code, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.ProductCode == code, ct);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
        {
            return await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync(ct);
        }
    }
}
