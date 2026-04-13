using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Home.Commands.CreateProductReview;

/// <summary>
/// Gửi đánh giá sản phẩm (tên tác giả, sao, nội dung) từ storefront.
/// </summary>
public class CreateProductReviewCommand : IRequest<Result<ProductReviewDto>>
{
    public Guid ProductId { get; set; }
    public string AuthorName { get; set; } = default!;
    public int Rating { get; set; }
    public string Comment { get; set; } = default!;
}
