using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminProduct;

/// <summary>
/// Tạo sản phẩm mới trong khu admin (có thể gắn danh mục).
/// </summary>
public class CreateAdminProductCommand : IRequest<Result<Guid>>
{
    public required AdminUpsertProductDto Dto { get; set; }
}
