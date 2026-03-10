using MediatR;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetPromoBanners;

/// <summary>
/// Query lấy danh sách banner khuyến mãi bên phải trang chủ.
/// </summary>
public class GetPromoBannersQuery : IRequest<List<HomeBanner>>
{
}

