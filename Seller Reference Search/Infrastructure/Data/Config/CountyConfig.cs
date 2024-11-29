using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller_Reference_Search.Infrastructure.Data.Models;

namespace Seller_Reference_Search.Infrastructure.Data.Config
{
    public class CountyConfig : IEntityTypeConfiguration<County>
    {
        public void Configure(EntityTypeBuilder<County> builder)
        {
            builder.ToTable("Counties"); 
            builder.Property(e => e.CountyName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.StateCode).IsRequired().HasMaxLength(2);
            builder.HasIndex(e => e.StateCode);
        }
    }
}
