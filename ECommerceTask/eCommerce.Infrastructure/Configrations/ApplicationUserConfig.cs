using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Infrastructure.Configrations
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(u => u.UserName)
                   .IsUnique();

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.LastLoginTime)
                   .HasColumnType("datetime2");
        }
    }
}
