namespace eCommerce.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categoryRepoMock = new Mock<ICategoryRepository>();

            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);

            _service = new CategoryService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCategory_WhenCategoryExists()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Electronics", Description = "Desc" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(1, default))
                             .ReturnsAsync(category);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Electronics", result!.Name);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenCategoryNameExists()
        {
            var dto = new CategoryCreateDto { Name = "Electronics", Description = "Desc" };
            _categoryRepoMock.Setup(r => r.GetByNameAsync("Electronics", default))
                             .ReturnsAsync(new Category());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
        }
    }
}
