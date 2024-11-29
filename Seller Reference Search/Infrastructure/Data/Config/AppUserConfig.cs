using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller_Reference_Search.Infrastructure.Data.Models;

namespace Seller_Reference_Search.Infrastructure.Data.Config
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(e => e.Id).UseIdentityColumn();
            builder.Property(e => e.Email).IsRequired();
            builder.Property(e => e.FirstName).HasMaxLength(50);
            builder.Property(e => e.LastName).HasMaxLength(50);
            builder.Property(e => e.Address1).HasMaxLength(100);
            builder.Property(e => e.Address2).HasMaxLength(100);
            builder.Property(e => e.City).HasMaxLength(50);
            builder.Property(e => e.State).HasMaxLength(50);
            builder.Property(e => e.CountryCode).HasMaxLength(3);
            builder.Property(e => e.PostalCode).HasMaxLength(20);
            builder.Property(e => e.ProfilePictureUrl).HasMaxLength(200);
            builder.Property(e => e.Gender).HasMaxLength(1);
            builder.Property(e => e.CreatedDate).HasDefaultValueSql("NOW()");
            builder.Property(e => e.IsActive).HasDefaultValue(true);
        }
    }
}
