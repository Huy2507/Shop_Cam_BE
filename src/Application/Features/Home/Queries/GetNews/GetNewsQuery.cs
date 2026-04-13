using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNews;

/// <summary>
/// Query danh sách tin tức có phân trang (trang chủ / trang blog).
/// </summary>
public class GetNewsQuery : IRequest<NewsFeedResult>
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
