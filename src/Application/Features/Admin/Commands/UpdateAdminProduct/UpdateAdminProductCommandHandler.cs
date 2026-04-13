using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminProduct;

/// <summary>
/// Tìm sản phẩm theo id, kiểm tra danh mục nếu đổi, cập nhật các trường hiển thị/giá.
/// </summary>
public class UpdateAdminProductCommandHandler : IRequestHandler<UpdateAdminProductCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public UpdateAdminProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateAdminProductCommand request, CancellationToken cancellationToken)
    {
        var d = request.Dto;
        if (string.IsNullOrWhiteSpace(d.Name))
            return Result<Unit>.Failure(ErrorCodes.INVALID_DATA);
        if (string.IsNullOrWhiteSpace(d.ImageUrl))
            return Result<Unit>.Failure(ErrorCodes.INVALID_DATA);

        var p = await _context.Products.FirstOrDefaultAsync(
            x => x.ProductId == request.ProductId,
            cancellationToken);
        if (p == null)
            return Result<Unit>.Failure(ErrorCodes.NOT_FOUND);

        if (d.ProductCategoryId.HasValue)
        {
            var exists = await _context.ProductCategories
                .AnyAsync(c => c.ProductCategoryId == d.ProductCategoryId && c.IsActive, cancellationToken);
            if (!exists)
                return Result<Unit>.Failure(ErrorCodes.NOT_FOUND);
        }

        p.Name = d.Name.Trim();
        p.Price = d.Price;
        p.Discount = d.Discount;
        p.Info = string.IsNullOrWhiteSpace(d.Info) ? null : d.Info.Trim();
        p.Description = string.IsNullOrWhiteSpace(d.Description) ? null : d.Description.Trim();
        p.ImageUrl = d.ImageUrl.Trim();
        p.IsNew = d.IsNew;
        p.OutOfStock = d.OutOfStock;
        p.Badge = string.IsNullOrWhiteSpace(d.Badge) ? null : d.Badge.Trim();
        p.ProductCategoryId = d.ProductCategoryId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
