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
        });
    }
}
