using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller_Reference_Search.Infrastructure.Data.Models;

namespace Seller_Reference_Search.Infrastructure.Data.Config
{
    public class SaleConfig : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable(nameof(Sale) + "s");
            builder.Property(e => e.Id).UseIdentityColumn();

            builder.Property(e => e.Reference)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.OwnerName)
                .HasMaxLength(500);

            builder.Property(e => e.ParcelNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LotAcreage)
                .IsRequired()
                .HasColumnType("double precision");

            builder.Property(e => e.OfferPrice)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(e => e.OfferPPA)
                .HasColumnType("decimal(18, 2)");

            builder.Property(e => e.RealPPA)
                .HasColumnType("decimal(18, 2)");

            builder.Property(e => e.PPACalc)
                .HasColumnType("decimal(18, 2)");

            builder.Property(e => e.Profit)
                .HasColumnType("decimal(18, 2)");

            builder.Property(e => e.RetailValue)
                .HasColumnType("decimal(18, 2)");

            builder.Property(e => e.County)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.State)
                .IsRequired()
                .HasMaxLength(2); // Assuming 2-letter state codes

            builder.Property(e => e.ZipCode)
                .HasMaxLength(10); // Accommodates ZIP+4 format

            builder.Property(e => e.ClosingDate)
                .HasColumnType("timestamp");

            builder.Property(e => e.FileUploadId)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Property(e => e.LastModifiedAt)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.HasIndex(e => e.Reference)
                .IsUnique()
                .HasDatabaseName("Uniq_Sales_Reference");

            builder.HasIndex(e => new
            {
                e.ParcelNumber,
                e.LotAcreage,
                e.OfferPrice,
                e.County,
                e.State,
                e.ClosingDate,
                e.FileUploadId
            }).HasDatabaseName("Search_Sales_ParcelNumber_LotAcreage_OfferPrice_County_State_ClosingDate_FileUploadId");

            builder.HasOne(e => e.FileUpload)
                .WithMany(e => e.Sales)
                .HasForeignKey(e => e.FileUploadId)
                .HasConstraintName("FK_Sales_FileUploads_FileUploadId");
        }
    }
}
