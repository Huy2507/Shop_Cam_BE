using MediatR;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminBanner;

/// <summary>
/// Validate URL ảnh rồi thêm HomeBanners.
/// </summary>
public class CreateAdminBannerCommandHandler : IRequestHandler<CreateAdminBannerCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateAdminBannerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateAdminBannerCommand request, CancellationToken cancellationToken)
    {
        var d = request.Dto;
        if (string.IsNullOrWhiteSpace(d.UrlImg))
            return Result<Guid>.Failure(ErrorCodes.INVALID_DATA);

        var id = Guid.NewGuid();
        _context.HomeBanners.Add(new HomeBanner
        {
            HomeBannerId = id,
            UrlImg = d.UrlImg.Trim(),
            Title = string.IsNullOrWhiteSpace(d.Title) ? null : d.Title.Trim(),
            Link = string.IsNullOrWhiteSpace(d.Link) ? null : d.Link.Trim(),
            IsMain = d.IsMain,
            DisplayOrder = d.DisplayOrder,
        });
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(id);
    }
}
