using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data.Config;
using Seller_Reference_Search.Infrastructure.Data.Models;
using System.Reflection;

namespace Seller_Reference_Search.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> ops) : base(ops)
        {

        }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<FileUpload> FileUploads { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<County> Counties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AppUserConfig());
            modelBuilder.ApplyConfiguration(new FileUploadConfig());
            modelBuilder.ApplyConfiguration(new SaleConfig());
            modelBuilder.ApplyConfiguration(new CountyConfig());
        }

        public string GetTableName<TEntity>() where TEntity : class
        {
            var entityType = Model.FindEntityType(typeof(TEntity));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            return $"{schema}.{tableName}";
        }
    }
}
