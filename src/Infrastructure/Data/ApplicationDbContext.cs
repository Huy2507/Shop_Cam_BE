using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shop_Cam_BE.Application.Common.Helpers;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Domain.Common;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : this(options, null)
    {
    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor? httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<User> Users { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<HomeBanner> HomeBanners { get; set; }
    public DbSet<NewsArticle> NewsArticles { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<SiteSetting> SiteSettings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<UserNewsPagePreference> UserNewsPagePreferences { get; set; }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var userId = TryGetCurrentUserId();

        foreach (var entry in ChangeTracker.Entries<IAuditableSoftDeletable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedTime = now;
                    entry.Entity.UpdatedTime = now;
                    if (userId.HasValue)
                    {
                        entry.Entity.CreatedByUserId = userId;
                        entry.Entity.UpdatedByUserId = userId;
                    }
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedTime = now;
                    if (userId.HasValue)
                        entry.Entity.UpdatedByUserId = userId;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private Guid? TryGetCurrentUserId()
    {
        var token = _httpContextAccessor?.HttpContext?.Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
            return null;
        var sub = JwtHelper.ExtractSubFromJwt(token);
        return Guid.TryParse(sub, out var id) ? id : null;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.HasIndex(e => e.IsActive);
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
            entity.HasIndex(e => e.IsActive);
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
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<HomeBanner>(entity =>
        {
            entity.HasKey(e => e.HomeBannerId);
            entity.Property(e => e.UrlImg)
                .IsRequired()
                .HasMaxLength(1000);
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<NewsArticle>(entity =>
        {
            entity.HasKey(e => e.NewsArticleId);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Body)
                .HasMaxLength(16000);
            entity.HasIndex(e => e.IsActive);
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
            entity.HasIndex(e => e.IsActive);
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

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(e => e.SiteSettingId);
            entity.Property(e => e.Group)
                .IsRequired()
                .HasMaxLength(64);
            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(e => e.ValueJson)
                .IsRequired();
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.HasIndex(e => new { e.Group, e.Key }).IsUnique();
            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(e => e.NormalizedName)
                .IsRequired()
                .HasMaxLength(128);
            entity.HasIndex(e => e.NormalizedName).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserNewsPagePreference>(entity =>
        {
            entity.HasKey(e => e.UserNewsPagePreferenceId);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.ValueJson)
                .IsRequired();
        });
    }
}
