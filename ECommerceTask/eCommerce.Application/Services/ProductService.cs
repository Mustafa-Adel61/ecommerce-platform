namespace eCommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHelper _fileHelper;

        public ProductService(IUnitOfWork unitOfWork, IFileHelper fileHelper)
        {
            _unitOfWork = unitOfWork;
            _fileHelper = fileHelper;
        }

        public async Task<PagedResult<ProductReadDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = await _unitOfWork.Products.ListWithCategoryAsync(ct);

            var totalCount = query.Count();

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Code = p.ProductCode,
                    Name = p.Name,
                    ImagePath = p.ImagePath,
                    Price = p.Price,
                    MinimumQuantity = p.MinimumQuantity,
                    DiscountRate = p.DiscountRate,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.Name
                })
                .ToList();

            return new PagedResult<ProductReadDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<ProductReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id, ct);
            if (product == null) return null;

            return new ProductReadDto
            {
                Id = product.Id,
                Code = product.ProductCode,
                Name = product.Name,
                ImagePath = product.ImagePath,
                Price = product.Price,
                MinimumQuantity = product.MinimumQuantity,
                DiscountRate = product.DiscountRate,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }


        public async Task<ProductReadDto?> GetByCodeAsync(string code, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products.GetByCodeAsync(code, ct);
            if (product == null) return null;

            return new ProductReadDto
            {
                Id = product.Id,
                Code = product.ProductCode,
                Name = product.Name,
                ImagePath = product.ImagePath,
                Price = product.Price,
                MinimumQuantity = product.MinimumQuantity,
                DiscountRate = product.DiscountRate,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }

        public async Task<IEnumerable<ProductReadDto>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
        {
            var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId, ct);

            return products.Select(p => new ProductReadDto
            {
                Id = p.Id,
                Code = p.ProductCode,
                Name = p.Name,
                ImagePath = p.ImagePath,
                Price = p.Price,
                MinimumQuantity = p.MinimumQuantity,
                DiscountRate = p.DiscountRate,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty
            });
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto, string rootPath, CancellationToken ct = default)
        {
            var existing = await _unitOfWork.Products.GetByCodeAsync(dto.Code, ct);
            if (existing != null)
                throw new InvalidOperationException("Product code already exists.");

            string? imagePath = null;
            if (dto.ImageFile != null)
            {
                imagePath = await _fileHelper.SaveImageAsync(dto.ImageFile, rootPath, "uploads/products");
            }

            var product = new Product
            {
                ProductCode = dto.Code,
                Name = dto.Name,
                ImagePath = imagePath,
                Price = dto.Price,
                MinimumQuantity = dto.MinimumQuantity,
                DiscountRate = dto.DiscountRate,
                CategoryId = dto.CategoryId
            };

            await _unitOfWork.Products.AddAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var productWithCategory = await _unitOfWork.Products.GetByIdWithCategoryAsync(product.Id, ct);

            return new ProductReadDto
            {
                Id = productWithCategory!.Id,
                Code = productWithCategory.ProductCode,
                Name = productWithCategory.Name,
                ImagePath = productWithCategory.ImagePath,
                Price = productWithCategory.Price,
                MinimumQuantity = productWithCategory.MinimumQuantity,
                DiscountRate = productWithCategory.DiscountRate,
                CategoryId = productWithCategory.CategoryId,
                CategoryName = productWithCategory.Category!.Name
            };
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto, string rootPath, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, ct);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.MinimumQuantity = dto.MinimumQuantity;
            product.DiscountRate = dto.DiscountRate;
            product.CategoryId = dto.CategoryId;

            if (dto.ImageFile != null)
            {
                // delete old image if exists
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    _fileHelper.DeleteImage(rootPath, product.ImagePath);
                }

                // save new image
                product.ImagePath = await _fileHelper.SaveImageAsync(dto.ImageFile, rootPath, "uploads/products");
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, ct);
            if (product == null) return false;

            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                _fileHelper.DeleteImage("wwwroot", product.ImagePath);
            }

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }
    }
}
