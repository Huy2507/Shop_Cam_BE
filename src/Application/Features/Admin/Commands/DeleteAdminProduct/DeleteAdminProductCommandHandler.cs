using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminProduct;

/// <summary>
/// Chặn xóa khi sản phẩm đã có trong đơn hàng (trả mã lỗi nghiệp vụ tương ứng).
/// </summary>
public class DeleteAdminProductCommandHandler : IRequestHandler<DeleteAdminProductCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteAdminProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(DeleteAdminProductCommand request, CancellationToken cancellationToken)
    {
        var p = await _context.Products
            .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);
        if (p == null)
            return Result<Unit>.Failure(ErrorCodes.NOT_FOUND);

        var inOrders = await _context.OrderItems
            .AnyAsync(i => i.ProductId == request.ProductId, cancellationToken);
        if (inOrders)
            return Result<Unit>.Failure(ErrorCodes.PRODUCT_DELETE_CONFLICT_ORDERS);

        p.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
