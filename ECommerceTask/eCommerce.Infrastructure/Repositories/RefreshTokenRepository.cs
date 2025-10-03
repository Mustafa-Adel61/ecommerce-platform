namespace eCommerce.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token, ct);
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.RefreshTokens
                .Where(r => r.UserId == userId)
                .ToListAsync(ct);
        }

        public void Update(RefreshToken token) => _context.RefreshTokens.Update(token);

        public async Task SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
    }
}
