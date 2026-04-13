using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminBanner;

/// <summary>
/// Tìm banner, kiểm tra ImageUrl không rỗng, cập nhật các trường.
/// </summary>
public class UpdateAdminBannerCommandHandler : IRequestHandler<UpdateAdminBannerCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;

    public UpdateAdminBannerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateAdminBannerCommand request, CancellationToken cancellationToken)
    {
        var d = request.Dto;
        if (string.IsNullOrWhiteSpace(d.UrlImg))
            return Result<Unit>.Failure(ErrorCodes.INVALID_DATA);

        var b = await _context.HomeBanners
            .FirstOrDefaultAsync(x => x.HomeBannerId == request.BannerId, cancellationToken);
        if (b == null)
            return Result<Unit>.Failure(ErrorCodes.NOT_FOUND);

        b.UrlImg = d.UrlImg.Trim();
        b.Title = string.IsNullOrWhiteSpace(d.Title) ? null : d.Title.Trim();
        b.Link = string.IsNullOrWhiteSpace(d.Link) ? null : d.Link.Trim();
        b.IsMain = d.IsMain;
        b.DisplayOrder = d.DisplayOrder;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
