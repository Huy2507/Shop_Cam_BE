using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Commands.CreateProductReview;

public class CreateProductReviewCommandHandler : IRequestHandler<CreateProductReviewCommand, Result<ProductReviewDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateProductReviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductReviewDto>> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
    {
        var name = request.AuthorName?.Trim() ?? string.Empty;
        var comment = request.Comment?.Trim() ?? string.Empty;

        if (name.Length is < 1 or > 100)
            return Result<ProductReviewDto>.Failure(ErrorCodes.INVALID_DATA, "Tên hiển thị từ 1 đến 100 ký tự.");
        if (comment.Length is < 1 or > 2000)
            return Result<ProductReviewDto>.Failure(ErrorCodes.INVALID_DATA, "Nội dung đánh giá từ 1 đến 2000 ký tự.");
        if (request.Rating is < 1 or > 5)
            return Result<ProductReviewDto>.Failure(ErrorCodes.INVALID_DATA, "Đánh giá từ 1 đến 5 sao.");

        var productExists = await _context.Products.AnyAsync(p => p.ProductId == request.ProductId, cancellationToken);
        if (!productExists)
            return Result<ProductReviewDto>.Failure(ErrorCodes.NOT_FOUND, "Sản phẩm không tồn tại.");

        var now = DateTime.UtcNow;
        var entity = new ProductReview
        {
            ProductReviewId = Guid.NewGuid(),
            ProductId = request.ProductId,
            AuthorName = name,
            Rating = (byte)request.Rating,
            Comment = comment,
            CreatedAt = now,
            IsApproved = true,
        };

        await _context.ProductReviews.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new ProductReviewDto
        {
            Id = entity.ProductReviewId,
            AuthorName = entity.AuthorName,
            Rating = entity.Rating,
            Comment = entity.Comment,
            CreatedAt = entity.CreatedAt,
        };

        return Result<ProductReviewDto>.Success(dto);
    }
}
