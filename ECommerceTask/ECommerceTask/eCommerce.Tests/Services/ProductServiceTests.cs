namespace eCommerce.Tests.Services
{
    using Moq;
    using Xunit;
    using eCommerce.Application.Services;
    using Microsoft.AspNetCore.Http;
    using eCommerce.Application.Common;

    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IFileHelper> _fileHelperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepoMock = new Mock<IProductRepository>();
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);

            // Mock FileHelper
            _fileHelperMock = new Mock<IFileHelper>();
            _fileHelperMock.Setup(f => f.SaveImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync("fakepath.jpg");

            // Inject mocks into ProductService
            _service = new ProductService(_unitOfWorkMock.Object, _fileHelperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnProductReadDto_WhenSuccessful()
        {
            // Arrange
            var dto = new ProductCreateDto { Code = "P01", Name = "Laptop", CategoryId = 1 };

            _productRepoMock.Setup(r => r.GetByCodeAsync("P01", default))
                            .ReturnsAsync((Product?)null);

            _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), default))
                            .Callback<Product, CancellationToken>((p, ct) => p.Id = 1)
                            .Returns(Task.CompletedTask);

            var createdProduct = new Product
            {
                Id = 1,
                ProductCode = "P01",
                Name = "Laptop",
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Electronics" },
                ImagePath = "fakepath.jpg"
            };
            _productRepoMock.Setup(r => r.GetByIdWithCategoryAsync(1, default))
                            .ReturnsAsync(createdProduct);

            // Act
            var result = await _service.CreateAsync(dto, "wwwroot");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laptop", result.Name);
            Assert.Equal("fakepath.jpg", result.ImagePath);
            Assert.Equal("Electronics", result.CategoryName);
        }

    }
}
