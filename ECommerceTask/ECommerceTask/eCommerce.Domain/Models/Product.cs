namespace eCommerce.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string ProductCode { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public decimal Price { get; set; }
        public int MinimumQuantity { get; set; }
        public decimal DiscountRate { get; set; }
    }
}
