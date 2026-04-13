using MediatR;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Home.Queries.GetCategories;

/// <summary>Danh sách danh mục hiển thị cho menu storefront.</summary>
public class GetCategoriesQuery : IRequest<List<ProductCategory>>
{
}

