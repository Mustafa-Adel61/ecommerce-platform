namespace eCommerce.Tests.Integration
{
    public class CategoryServiceIntegrationTests
    {
        private readonly CategoryService _service;
        private readonly ApplicationDbContext _context;

        public CategoryServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed data
            _context.Categories.Add(new Category { Id = 1, Name = "Electronics", Description = "Desc" });
            _context.SaveChanges();

            var unitOfWork = new UnitOfWork(_context);
            _service = new CategoryService(unitOfWork);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCategories()
        {
            var result = await _service.GetAllAsync();
            Assert.Single(result);
        }

        [Fact]
        public async Task CreateAsync_AddsCategory()
        {
            var dto = new CategoryCreateDto { Name = "Clothes", Description = "Desc" };
            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Clothes", result.Name);

            var categories = await _service.GetAllAsync();
            Assert.Equal(2, categories.Count()); 
        }
    }
}
