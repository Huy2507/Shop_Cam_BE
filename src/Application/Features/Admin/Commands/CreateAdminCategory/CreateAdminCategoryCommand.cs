using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminCategory;

/// <summary>
/// Tạo danh mục sản phẩm (tên bắt buộc).
/// </summary>
public class CreateAdminCategoryCommand : IRequest<Result<Guid>>
{
    public required AdminUpsertCategoryDto Dto { get; set; }
}
