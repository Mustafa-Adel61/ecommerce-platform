namespace eCommerce.Application.Interfaces.IRepository
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId, CancellationToken ct = default);
        void Update(RefreshToken token);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
