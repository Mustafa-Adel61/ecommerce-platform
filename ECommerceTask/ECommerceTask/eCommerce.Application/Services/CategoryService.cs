namespace eCommerce.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryReadDto>> GetAllAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.Categories.ListAsync(ct);
            return categories.Select(c => new CategoryReadDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, ct);
            if (category == null) return null;

            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
        {
            var existing = await _unitOfWork.Categories.GetByNameAsync(dto.Name, ct);
            if (existing != null)
                throw new InvalidOperationException("Category name already exists.");

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _unitOfWork.Categories.AddAsync(category, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryReadDto?> UpdateAsync(int id, CategoryCreateDto dto, CancellationToken ct = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, ct);
            if (category == null) return null;

            category.Name = dto.Name;
            category.Description = dto.Description;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(ct);

            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, ct);
            if (category == null) return false;

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }
}
