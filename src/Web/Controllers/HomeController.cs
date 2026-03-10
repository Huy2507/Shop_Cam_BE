#nullable disable
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_Cam_BE.Application.Features.Home.Queries.GetBanners;
using Shop_Cam_BE.Application.Features.Home.Queries.GetNewProducts;
using Shop_Cam_BE.Application.Features.Home.Queries.GetNews;
using Shop_Cam_BE.Application.Features.Home.Queries.GetProducts;
using Shop_Cam_BE.Application.Features.Home.Queries.GetPromoBanners;

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
    /// Lấy danh sách banner chính cho trang chủ.
    /// Flow: Controller -> MediatR (GetBannersQuery) -> Handler -> DB -> map sang JSON FE cần.
    /// </summary>
    [HttpGet("banners")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBanners(CancellationToken cancellationToken)
    {
        // Gửi query vào Application layer để lấy dữ liệu.
        var banners = await _mediator.Send(new GetBannersQuery(), cancellationToken);

        // Map entity sang shape FE sử dụng: { id, urlimg, title, link }.
        return Ok(banners.Select(b => new
        {
            id = b.HomeBannerId,
            urlimg = b.UrlImg,
            title = b.Title,
            link = b.Link
        }));
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

        return Ok(banners.Select(b => new
        {
            id = b.HomeBannerId,
            urlimg = b.UrlImg,
            title = b.Title,
            link = b.Link
        }));
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

        return Ok(products.Select(p => new
        {
            id = p.ProductId,
            name = p.Name,
            price = p.Price,
            discount = p.Discount ?? 0m,
            info = p.Info ?? string.Empty,
            imageUrl = p.ImageUrl,
            isNew = p.IsNew,
            outOfStock = p.OutOfStock,
            badge = p.Badge ?? string.Empty
        }));
    }

    /// <summary>
    /// Lấy danh sách sản phẩm mới (IsNew = true) cho khu vực "Bộ sưu tập mới".
    /// </summary>
    [HttpGet("new-products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNewProducts(CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetNewProductsQuery(), cancellationToken);

        return Ok(products.Select(p => new
        {
            id = p.ProductId,
            name = p.Name,
            price = p.Price,
            discount = p.Discount,
            info = p.Info,
            imageUrl = p.ImageUrl,
            isNew = p.IsNew,
            outOfStock = p.OutOfStock,
            badge = p.Badge
        }));
    }

    /// <summary>
    /// Lấy danh sách tin tức mới nhất cho khu vực News.
    /// </summary>
    [HttpGet("news")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNews(CancellationToken cancellationToken)
    {
        var news = await _mediator.Send(new GetNewsQuery(), cancellationToken);

        return Ok(news.Select(n => new
        {
            id = n.NewsArticleId,
            title = n.Title,
            imageUrl = n.ImageUrl,
            excerpt = n.Excerpt ?? string.Empty,
            link = n.Link,
            publishedAt = n.PublishedAt
        }));
    }
}

