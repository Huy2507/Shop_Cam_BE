using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNewsById;

public class GetNewsByIdQuery : IRequest<NewsDetailDto?>
{
    public Guid NewsArticleId { get; set; }
}
