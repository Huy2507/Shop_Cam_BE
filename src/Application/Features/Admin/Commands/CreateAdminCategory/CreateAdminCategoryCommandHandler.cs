using MediatR;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Admin.Commands.CreateAdminCategory;

/// <summary>
/// Thêm bản ghi ProductCategories sau khi validate tên.
/// </summary>
public class CreateAdminCategoryCommandHandler : IRequestHandler<CreateAdminCategoryCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateAdminCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateAdminCategoryCommand request, CancellationToken cancellationToken)
    {
        var d = request.Dto;
        if (string.IsNullOrWhiteSpace(d.Name))
            return Result<Guid>.Failure(ErrorCodes.INVALID_DATA);

        var id = Guid.NewGuid();
        _context.ProductCategories.Add(new ProductCategory
        {
            ProductCategoryId = id,
            Name = d.Name.Trim(),
            Slug = string.IsNullOrWhiteSpace(d.Slug) ? null : d.Slug.Trim(),
        });
        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(id);
    }
}
