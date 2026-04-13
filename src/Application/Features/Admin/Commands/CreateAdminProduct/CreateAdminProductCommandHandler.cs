using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminProduct;

/// <summary>
/// Kiểm tra dữ liệu bắt buộc và ProductCategoryId (nếu có) rồi thêm bản ghi Products.
/// </summary>
public class CreateAdminProductCommandHandler : IRequestHandler<CreateAdminProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateAdminProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateAdminProductCommand request, CancellationToken cancellationToken)
    {
        var d = request.Dto;
        if (string.IsNullOrWhiteSpace(d.Name))
            return Result<Guid>.Failure(ErrorCodes.INVALID_DATA);
        if (string.IsNullOrWhiteSpace(d.ImageUrl))
            return Result<Guid>.Failure(ErrorCodes.INVALID_DATA);

        if (d.ProductCategoryId.HasValue)
        {
            var exists = await _context.ProductCategories
                .AnyAsync(c => c.ProductCategoryId == d.ProductCategoryId && c.IsActive, cancellationToken);
            if (!exists)
                return Result<Guid>.Failure(ErrorCodes.NOT_FOUND);
        }

        var id = Guid.NewGuid();
        _context.Products.Add(new Product
        {
            ProductId = id,
            Name = d.Name.Trim(),
            Price = d.Price,
            Discount = d.Discount,
            Info = string.IsNullOrWhiteSpace(d.Info) ? null : d.Info.Trim(),
            Description = string.IsNullOrWhiteSpace(d.Description) ? null : d.Description.Trim(),
            ImageUrl = d.ImageUrl.Trim(),
            IsNew = d.IsNew,
            OutOfStock = d.OutOfStock,
            Badge = string.IsNullOrWhiteSpace(d.Badge) ? null : d.Badge.Trim(),
            ProductCategoryId = d.ProductCategoryId,
        });
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(id);
    }
}
