namespace eCommerce.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public DateTime? LastLoginTime { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
