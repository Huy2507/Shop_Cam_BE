using MediatR;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetNewProducts;

/// <summary>
/// Query lấy danh sách sản phẩm mới (IsNew = true) cho khu vực "Bộ sưu tập mới".
/// </summary>
public class GetNewProductsQuery : IRequest<List<Product>>
{
}

