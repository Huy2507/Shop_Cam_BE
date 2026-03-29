using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetProductReviews;

public class GetProductReviewsQuery : IRequest<ProductReviewsResult>
{
    public Guid ProductId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
