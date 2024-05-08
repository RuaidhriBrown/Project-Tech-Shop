using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.Common;
using Project.Tech.Shop.Services.Products.Enitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.Products
{
    public class ProductsContext : DbContext, IUnitOfWork
    {
        private readonly ILogger<ProductsContext>? _logger;
        private const string SchemaName = "Products";

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }

        public ProductsContext()
        {
        }

        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
        {
        }

        public ProductsContext(DbContextOptions<ProductsContext> options, ILogger<ProductsContext> logger) : base(options)
        {
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Database=dev-tech-shop;Port=5432;User Id=dts-postgres;Password=X1B2#WXYZ123a;",
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", SchemaName));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema(SchemaName);

            // Configure the Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId); // Defines the primary key
                entity.Property(p => p.Name).HasMaxLength(256).IsRequired();
                entity.Property(p => p.Brand).HasMaxLength(256);
                entity.Property(p => p.Description).HasMaxLength(256);
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18, 2)"); // Ensuring decimal precision
                entity.Property(p => p.Condition);
                entity.Property(p => p.Category).IsRequired();
                entity.Property(p => p.StockLevel).IsRequired();
                entity.Property(p => p.Image).HasColumnType("text");

                // Assuming categories are limited and known, you might consider indexing on Category if queries often filter on this column
                entity.HasIndex(p => p.Category).HasName("IX_Products_Category");
            });

            // Configure the Sale entity
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sales");
                entity.HasKey(s => s.SaleId); // Primary key
                entity.Property(s => s.SaleDate).IsRequired();
                entity.Property(s => s.TotalSaleAmount).IsRequired().HasColumnType("decimal(18, 2)");
                entity.Property(s => s.Status).IsRequired().HasMaxLength(256);

                // Link to Basket instead of Product
                entity.HasOne(s => s.Basket)
                    .WithOne(b => b.Sale)
                    .HasForeignKey<Sale>(s => s.BasketId)
                    .OnDelete(DeleteBehavior.Restrict);  // Prevents cascade delete from removing the basket

                entity.Property(s => s.CustomerId).IsRequired();
            });

            // Basket Configuration
            modelBuilder.Entity<Basket>(entity =>
            {
                entity.ToTable("Baskets");
                entity.HasKey(b => b.BasketId);
                entity.Property(b => b.CustomerId).IsRequired();
                entity.Property(b => b.Status).IsRequired();  // Ensures each customer has only one active basket

            });

            // BasketItem Configuration
            modelBuilder.Entity<BasketItem>(entity =>
            {
                entity.ToTable("BasketItems");
                entity.HasKey(bi => bi.BasketItemId);
                entity.Property(bi => bi.Quantity).IsRequired();

                entity.HasOne(bi => bi.Basket)
                    .WithMany(b => b.Items)
                    .HasForeignKey(bi => bi.BasketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bi => bi.Product)
                    .WithMany(p => p.BasketItems)
                    .HasForeignKey(bi => bi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public async Task<UnitResult<UserDbErrorReason>> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await base.SaveChangesAsync(cancellationToken);
                return UnitResult.Success<UserDbErrorReason>();
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
            {
                _logger?.LogError("Error occurred during the entity update, the entity is not unique, exception: {Exception}", ex.ToString());
                return UnitResult.Failure(UserDbErrorReason.NotUnique);
            }
        }
    }
}
