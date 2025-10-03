namespace eCommerce.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _dbSet.FindAsync(new object[] { id }, ct);

        public virtual async Task<IEnumerable<T>> ListAsync(CancellationToken ct = default) =>
            await _dbSet.ToListAsync(ct);

        public virtual async Task AddAsync(T entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public virtual void Update(T entity) => _dbSet.Update(entity);

        public virtual void Remove(T entity) => _dbSet.Remove(entity);
    }
}
