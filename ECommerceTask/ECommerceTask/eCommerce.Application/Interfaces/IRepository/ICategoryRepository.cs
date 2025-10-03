namespace eCommerce.Application.Interfaces.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);
    }
}
