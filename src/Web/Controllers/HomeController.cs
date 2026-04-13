#nullable disable
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_Cam_BE.Application.Common.Mappings;
using Shop_Cam_BE.Application.Features.Home.Commands.CreateProductReview;
using Shop_Cam_BE.Application.Features.Home.Queries.GetBanners;
using Shop_Cam_BE.Application.Features.Home.Queries.GetCatalogProducts;
using Shop_Cam_BE.Application.Features.Home.Queries.GetCategories;
using Shop_Cam_BE.Application.Features.Home.Queries.GetNewProducts;
using Shop_Cam_BE.Application.Features.Home.Queries.GetNews;
using Shop_Cam_BE.Application.Features.Home.Queries.GetNewsById;
using Shop_Cam_BE.Application.Features.Home.Queries.GetProductById;
using Shop_Cam_BE.Application.Features.Home.Queries.GetProducts;
using Shop_Cam_BE.Application.Features.Home.Queries.GetPromoBanners;
using Shop_Cam_BE.Application.Features.Home.Queries.GetProductReviews;
using Shop_Cam_BE.Application.Features.Home.Queries.GetRelatedProducts;
using Shop_Cam_BE.Application.Features.SiteSettings.Queries.GetPublicUiSettings;

namespace Shop_Cam_BE.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy danh mục để hiển thị menu storefront.
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(categories.Select(HomeStorefrontResponseMapper.FromCategory));
    }

    /// <summary>
    /// Lấy danh sách banner chính cho trang chủ.
    /// Flow: Controller -> MediatR (GetBannersQuery) -> Handler -> DB -> map sang JSON FE cần.
    /// </summary>
    [HttpGet("banners")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBanners(CancellationToken cancellationToken)
    {
        // Gửi query vào Application layer để lấy dữ liệu.
        var banners = await _mediator.Send(new GetBannersQuery(), cancellationToken);

        return Ok(banners.Select(HomeStorefrontResponseMapper.FromBanner));
    }

    /// <summary>
    /// Lấy danh sách banner khuyến mãi (cột bên phải).
    /// Dữ liệu cũng đi qua CQRS giống GetBanners.
    /// </summary>
    [HttpGet("promo-banners")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPromoBanners(CancellationToken cancellationToken)
    {
        var banners = await _mediator.Send(new GetPromoBannersQuery(), cancellationToken);

        return Ok(banners.Select(HomeStorefrontResponseMapper.FromBanner));
    }

    /// <summary>
    /// Lấy danh sách sản phẩm cho khu vực Product List.
    /// Hỗ trợ filter query string: best | hot | combo.
    /// </summary>
    [HttpGet("products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts([FromQuery] string filter, CancellationToken cancellationToken)
    {
        // Toàn bộ điều kiện filter được xử lý trong Handler,
        // controller chỉ truyền đúng tham số filter xuống.
        var products = await _mediator.Send(new GetProductsQuery { Filter = filter }, cancellationToken);

        return Ok(products.Select(HomeStorefrontResponseMapper.FromProductForHomeList));
    }

    /// <summary>
    /// Lấy danh sách sản phẩm mới (IsNew = true) cho khu vực "Bộ sưu tập mới".
    /// </summary>
    [HttpGet("new-products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNewProducts(CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetNewProductsQuery(), cancellationToken);

        return Ok(products.Select(HomeStorefrontResponseMapper.FromProductForNewArrivals));
    }

    /// <summary>
    /// Lấy danh sách tin tức mới nhất cho khu vực News.
    /// </summary>
    [HttpGet("news")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetNewsQuery { Page = page, PageSize = pageSize },
            cancellationToken);

        return Ok(new
        {
            items = result.Items.Select(HomeStorefrontResponseMapper.FromNewsArticle),
            totalCount = result.TotalCount,
            page = result.Page,
            pageSize = result.PageSize,
            totalPages = result.TotalPages,
        });
    }

    /// <summary>
    /// Chi tiết một bài tin (PDP tin tức).
    /// </summary>
    [HttpGet("news/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNewsById(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetNewsByIdQuery { NewsArticleId = id }, cancellationToken);
        if (dto == null) return NotFound();
        return Ok(HomeStorefrontResponseMapper.FromNewsDetail(dto));
    }

    /// <summary>
    /// Chi tiết sản phẩm (PDP).
    /// </summary>
    [HttpGet("product/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var p = await _mediator.Send(new GetProductByIdQuery { ProductId = id }, cancellationToken);
        if (p == null) return NotFound();
        return Ok(HomeStorefrontResponseMapper.FromProductDetail(p));
    }

    /// <summary>
    /// Sản phẩm liên quan (cùng danh mục).
    /// </summary>
    [HttpGet("product/{id:guid}/related")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRelatedProducts(Guid id, CancellationToken cancellationToken)
    {
        var list = await _mediator.Send(new GetRelatedProductsQuery { ProductId = id, Take = 5 }, cancellationToken);
        return Ok(list.Select(HomeStorefrontResponseMapper.FromCatalogProduct));
    }

    /// <summary>
    /// Danh mục / tìm kiếm có phân trang (trang Mua hàng online).
    /// </summary>
    [HttpGet("catalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCatalog(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string q = null,
        [FromQuery] string category = null,
        [FromQuery] string sort = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string tab = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCatalogProductsQuery
        {
            Search = q,
            CategoryName = category,
            Sort = sort,
            Page = page,
            PageSize = pageSize,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            HomeTab = tab
        }, cancellationToken);

        return Ok(new
        {
            items = result.Items.Select(HomeStorefrontResponseMapper.FromCatalogProduct),
            totalCount = result.TotalCount,
            page = result.Page,
            pageSize = result.PageSize,
            totalPages = result.TotalPages
        });
    }

    [HttpGet("product/{id:guid}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductReviews(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetProductReviewsQuery { ProductId = id, Page = page, PageSize = pageSize },
            cancellationToken);

        return Ok(new
        {
            items = result.Items.Select(r => new
            {
                id = r.Id,
                authorName = r.AuthorName,
                rating = r.Rating,
                comment = r.Comment,
                createdAt = r.CreatedAt,
            }),
            totalCount = result.TotalCount,
            page = result.Page,
            pageSize = result.PageSize,
            totalPages = result.TotalPages,
            averageRating = result.AverageRating,
        });
    }

    [HttpPost("product/{id:guid}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateProductReview(
        Guid id,
        [FromBody] CreateProductReviewRequest body,
        CancellationToken cancellationToken = default)
    {
        if (body == null)
            return BadRequest();

        var result = await _mediator.Send(
            new CreateProductReviewCommand
            {
                ProductId = id,
                AuthorName = body.AuthorName ?? string.Empty,
                Rating = body.Rating,
                Comment = body.Comment ?? string.Empty,
            },
            cancellationToken);

        if (!result.Succeeded)
            return BadRequest(result);

        var v = result.Value!;
        return Ok(new
        {
            id = v.Id,
            authorName = v.AuthorName,
            rating = v.Rating,
            comment = v.Comment,
            createdAt = v.CreatedAt,
        });
    }

    /// <summary>
    /// Cấu hình giao diện + Zalo (OA, link dự phòng, lời chào) — storefront áp dụng; không cần đăng nhập.
    /// </summary>
    [HttpGet("ui-config")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUiConfig(CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetPublicUiSettingsQuery(), cancellationToken);
        return Ok(dto);
    }
}

public class CreateProductReviewRequest
{
    public string AuthorName { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
}
