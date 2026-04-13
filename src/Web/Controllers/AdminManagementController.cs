using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Application.DTOs;
using Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminBanner;
using Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminCategory;
using Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminProduct;
using Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminBanner;
using Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminCategory;
using Shop_Cam_BE.Application.Features.Admin.Commands.DeleteAdminProduct;
using Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminBanner;
using Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminCategory;
using Shop_Cam_BE.Application.Features.Admin.Commands.UpdateAdminProduct;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminBannerById;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminBannersPaged;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoriesLookup;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoriesPaged;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminCategoryById;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminOrderById;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminOrdersPaged;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminProductById;
using Shop_Cam_BE.Application.Features.Admin.Queries.GetAdminProductsPaged;

namespace Shop_Cam_BE.Web.Controllers;

/// <summary>CRUD quản trị: sản phẩm, danh mục, banner, xem đơn hàng — chỉ role Admin.</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Policy = "Admin")]
public class AdminManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- Products ---

    [HttpGet("products")]
    public async Task<ActionResult<PagedResult<AdminProductListItemDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAdminProductsPagedQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                CategoryId = categoryId,
            },
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("products/{id:guid}")]
    public async Task<ActionResult<AdminProductDetailDto>> GetProduct(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetAdminProductByIdQuery { ProductId = id }, cancellationToken);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] AdminUpsertProductDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAdminProductCommand { Dto = dto }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] AdminUpsertProductDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAdminProductCommand { ProductId = id, Dto = dto }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteAdminProductCommand { ProductId = id }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    // --- Categories ---

    [HttpGet("categories/lookup")]
    public async Task<ActionResult<List<AdminCategoryLookupDto>>> GetCategoriesLookup(CancellationToken cancellationToken)
    {
        var list = await _mediator.Send(new GetAdminCategoriesLookupQuery(), cancellationToken);
        return Ok(list);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<PagedResult<AdminCategoryListItemDto>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAdminCategoriesPagedQuery { Page = page, PageSize = pageSize, Search = search },
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("categories/{id:guid}")]
    public async Task<ActionResult<AdminCategoryDetailDto>> GetCategory(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetAdminCategoryByIdQuery { CategoryId = id }, cancellationToken);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] AdminUpsertCategoryDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAdminCategoryCommand { Dto = dto }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return CreatedAtAction(nameof(GetCategory), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] AdminUpsertCategoryDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAdminCategoryCommand { CategoryId = id, Dto = dto }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteAdminCategoryCommand { CategoryId = id }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    // --- Banners ---

    [HttpGet("banners")]
    public async Task<ActionResult<PagedResult<AdminBannerListItemDto>>> GetBanners(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAdminBannersPagedQuery { Page = page, PageSize = pageSize, Search = search },
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("banners/{id:guid}")]
    public async Task<ActionResult<AdminBannerListItemDto>> GetBanner(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetAdminBannerByIdQuery { BannerId = id }, cancellationToken);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost("banners")]
    public async Task<IActionResult> CreateBanner([FromBody] AdminUpsertBannerDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAdminBannerCommand { Dto = dto }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return CreatedAtAction(nameof(GetBanner), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("banners/{id:guid}")]
    public async Task<IActionResult> UpdateBanner(Guid id, [FromBody] AdminUpsertBannerDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAdminBannerCommand { BannerId = id, Dto = dto }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("banners/{id:guid}")]
    public async Task<IActionResult> DeleteBanner(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteAdminBannerCommand { BannerId = id }, cancellationToken);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    // --- Orders (read-only) ---

    [HttpGet("orders")]
    public async Task<ActionResult<PagedResult<AdminOrderListItemDto>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAdminOrdersPagedQuery { Page = page, PageSize = pageSize, Search = search },
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("orders/{id:guid}")]
    public async Task<ActionResult<AdminOrderDetailDto>> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetAdminOrderByIdQuery { OrderId = id }, cancellationToken);
        if (dto == null) return NotFound();
        return Ok(dto);
    }
}
