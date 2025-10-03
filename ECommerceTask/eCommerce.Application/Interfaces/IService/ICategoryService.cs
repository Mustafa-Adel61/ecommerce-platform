using eCommerce.Application.DTOs.Category;
using eCommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Application.Interfaces.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryReadDto>> GetAllAsync(CancellationToken ct = default);
        Task<CategoryReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default);
        Task<CategoryReadDto?> UpdateAsync(int id, CategoryCreateDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
