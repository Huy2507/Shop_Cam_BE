using MediatR;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetBanners;

/// <summary>
/// Query lấy danh sách banner chính trên trang chủ.
/// Controller sẽ chịu trách nhiệm map sang shape JSON FE cần.
/// </summary>
public class GetBannersQuery : IRequest<List<HomeBanner>>
{
}

