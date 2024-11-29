using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller_Reference_Search.Infrastructure.Data.Models;

namespace Seller_Reference_Search.Infrastructure.Data.Config
{
    public class FileUploadConfig : IEntityTypeConfiguration<FileUpload>
    {
        public void Configure(EntityTypeBuilder<FileUpload> builder)
        {
            builder.ToTable(nameof(FileUpload) + "s");    
            builder.Property(e => e.Id).UseIdentityColumn();
            builder.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            builder.Property(e => e.FileSize).IsRequired();
            builder.Property(e => e.Status).IsRequired().HasMaxLength(50);
            builder.Property(e => e.UploadedByUserId).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            builder.Property(e => e.Description).HasMaxLength(4000);
            builder.Property(e => e.LastModified).HasDefaultValueSql("NOW()");
            builder.Property(e => e.UploadPath).HasMaxLength(1000);
            builder.Property(e => e.Checksum).HasMaxLength(64); //SHA-256
            builder.Property(e => e.UploadDuration);
            builder.Property(e => e.Notes).HasMaxLength(1000);

            builder.HasOne(e => e.AppUser)
                .WithMany(e => e.FileUploads)
                .HasForeignKey(e => e.UploadedByUserId)
                .HasConstraintName("FK_FileUploads_AspNetUsers_UploadedByUserId");
        }
    }
}
