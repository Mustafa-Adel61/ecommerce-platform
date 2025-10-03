namespace eCommerce.Application.Interfaces.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Product>> ListWithCategoryAsync(CancellationToken ct = default);
        Task<Product?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);
    }
}
