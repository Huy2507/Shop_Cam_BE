using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<HomeBanner> HomeBanners { get; set; }
    public DbSet<NewsArticle> NewsArticles { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.KeycloakId).IsUnique();
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description)
                .HasMaxLength(8000);
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.ProductCategoryId);
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId);
            entity.Property(e => e.AuthorName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Comment)
                .IsRequired()
                .HasMaxLength(2000);
            entity.Property(e => e.Rating)
                .HasColumnType("tinyint");
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ProductId);
        });

        modelBuilder.Entity<HomeBanner>(entity =>
        {
            entity.HasKey(e => e.HomeBannerId);
            entity.Property(e => e.UrlImg)
                .IsRequired()
                .HasMaxLength(1000);
        });

        modelBuilder.Entity<NewsArticle>(entity =>
        {
            entity.HasKey(e => e.NewsArticleId);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Body)
                .HasMaxLength(16000);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(200);
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId);
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(300);
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18,2)");
            entity.Property(e => e.LineTotal)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId);
        });
    }
}
