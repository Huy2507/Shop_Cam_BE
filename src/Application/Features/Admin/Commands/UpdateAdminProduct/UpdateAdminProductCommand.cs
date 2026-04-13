using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminProduct;

/// <summary>
/// Cập nhật sản phẩm theo ProductId và DTO chỉnh sửa.
/// </summary>
public class UpdateAdminProductCommand : IRequest<Result<Unit>>
{
    public Guid ProductId { get; set; }
    public required AdminUpsertProductDto Dto { get; set; }
}
