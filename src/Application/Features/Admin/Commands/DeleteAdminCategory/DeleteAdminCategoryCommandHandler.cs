using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminCategory;

/// <summary>
/// Kiểm tra còn sản phẩm tham chiếu danh mục trước khi xóa mềm (IsActive = false).
/// </summary>
public class DeleteAdminCategoryCommandHandler : IRequestHandler<DeleteAdminCategoryCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteAdminCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(DeleteAdminCategoryCommand request, CancellationToken cancellationToken)
    {
        var c = await _context.ProductCategories
            .FirstOrDefaultAsync(x => x.ProductCategoryId == request.CategoryId, cancellationToken);
        if (c == null)
            return Result<Unit>.Failure(ErrorCodes.NOT_FOUND);

        var hasProducts = await _context.Products
            .AnyAsync(p => p.ProductCategoryId == request.CategoryId, cancellationToken);
        if (hasProducts)
            return Result<Unit>.Failure(ErrorCodes.CATEGORY_DELETE_HAS_PRODUCTS);

        c.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
