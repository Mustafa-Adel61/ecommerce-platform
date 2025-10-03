using eCommerce.Application.Common;
using eCommerce.Application.DTOs.Product;

namespace eCommerce.Application.Interfaces.IService
{
    public interface IProductService
    {
        Task<PagedResult<ProductReadDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<ProductReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProductReadDto?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<IEnumerable<ProductReadDto>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);

        Task<ProductReadDto> CreateAsync(ProductCreateDto dto, string rootPath, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, ProductUpdateDto dto, string rootPath, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
