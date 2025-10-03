namespace eCommerce.Application.DTOs.Product
{
    public class ProductUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal DiscountRate { get; set; }
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
