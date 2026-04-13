using MediatR;
using Shop_Cam_BE.Application.DTOs;

namespace Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminSummary;

/// <summary>
/// Thống kê nhanh số lượng entity chính cho dashboard admin.
/// </summary>
public class GetAdminSummaryQuery : IRequest<AdminSummaryDto>
{
}
