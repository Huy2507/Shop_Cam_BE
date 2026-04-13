using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNewsById;

/// <summary>
/// Chi tiết một bài tin theo id (null nếu không có).
/// </summary>
public class GetNewsByIdQuery : IRequest<NewsDetailDto?>
{
    public Guid NewsArticleId { get; set; }
}
