using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminCategory;

/// <summary>
/// Tìm danh mục và ghi đè tên sau khi trim.
/// </summary>
public class UpdateAdminCategoryCommandHandler : IRequestHandler<UpdateAdminCategoryCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public UpdateAdminCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateAdminCategoryCommand request, CancellationToken cancellationToken)
    {
        var d = request.Dto;
        if (string.IsNullOrWhiteSpace(d.Name))
            return Result<Unit>.Failure(ErrorCodes.INVALID_DATA);

        var c = await _context.ProductCategories
            .FirstOrDefaultAsync(x => x.ProductCategoryId == request.CategoryId, cancellationToken);
        if (c == null)
            return Result<Unit>.Failure(ErrorCodes.NOT_FOUND);

        c.Name = d.Name.Trim();
        c.Slug = string.IsNullOrWhiteSpace(d.Slug) ? null : d.Slug.Trim();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
