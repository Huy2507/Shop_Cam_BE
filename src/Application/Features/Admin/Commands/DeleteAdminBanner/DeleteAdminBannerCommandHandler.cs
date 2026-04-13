using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminBanner;

/// <summary>
/// Xóa mềm banner (IsActive = false).
/// </summary>
public class DeleteAdminBannerCommandHandler : IRequestHandler<DeleteAdminBannerCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public DeleteAdminBannerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(DeleteAdminBannerCommand request, CancellationToken cancellationToken)
    {
        var b = await _context.HomeBanners
            .FirstOrDefaultAsync(x => x.HomeBannerId == request.BannerId, cancellationToken);
        if (b == null)
            return Result<Unit>.Failure(ErrorCodes.NOT_FOUND);

        b.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
