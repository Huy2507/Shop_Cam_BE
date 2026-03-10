using MediatR;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNews;

/// <summary>
/// Query lấy danh sách bài viết / tin tức mới nhất cho khu vực News trên trang chủ.
/// </summary>
public class GetNewsQuery : IRequest<List<NewsArticle>>
{
}

