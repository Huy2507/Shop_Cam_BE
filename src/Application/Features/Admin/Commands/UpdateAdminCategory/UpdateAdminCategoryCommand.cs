using MediatR;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminCategory;

/// <summary>
/// Cập nhật tên danh mục theo CategoryId.
/// </summary>
public class UpdateAdminCategoryCommand : IRequest<Result<Unit>>
{
    public Guid CategoryId { get; set; }
    public required AdminUpsertCategoryDto Dto { get; set; }
}
