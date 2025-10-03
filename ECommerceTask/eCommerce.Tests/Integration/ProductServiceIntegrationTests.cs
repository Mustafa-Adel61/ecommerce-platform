using eCommerce.Tests.Helpers;

namespace eCommerce.Tests.Integration
{
    public class ProductServiceIntegrationTests
    {
        private readonly ProductService _service;
        private readonly ApplicationDbContext _context;

        public ProductServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed categories
            _context.Categories.Add(new Category { Id = 1, Name = "Electronics" });
            _context.SaveChanges();

            var unitOfWork = new UnitOfWork(_context);
            var fakeFileHelper = new FakeFileHelper();

            _service = new ProductService(unitOfWork, fakeFileHelper);
        }


        [Fact]
        public async Task CreateAsync_AddsProduct()
        {
            var dto = new ProductCreateDto
            {
                Code = "P01",
                Name = "Laptop",
                Price = 1000,
                MinimumQuantity = 1,
                DiscountRate = 5,
                CategoryId = 1
            };

            var result = await _service.CreateAsync(dto, "wwwroot");

            Assert.NotNull(result);
            Assert.Equal("Laptop", result.Name);
            Assert.Equal("Electronics", result.CategoryName);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsPagedResults()
        {
            var result = await _service.GetAllAsync(1, 10);

            Assert.NotNull(result);
            Assert.True(result.Items.Count() <= 10);
        }
    }
}
