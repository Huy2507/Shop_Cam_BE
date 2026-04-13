using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop_Cam_BE.Application.Common;
using Shop_Cam_BE.Application.Common.Constants;
using Shop_Cam_BE.Application.Common.Interfaces;
using Shop_Cam_BE.Application.Common.Models;
using Shop_Cam_BE.Domain.Entities;

namespace Shop_Cam_BE.Application.Features.Users.Commands.UpsertUserNewsPagePreference;

public class UpsertUserNewsPagePreferenceCommandHandler : IRequestHandler<UpsertUserNewsPagePreferenceCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpsertUserNewsPagePreferenceCommandHandler(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(UpsertUserNewsPagePreferenceCommand request, CancellationToken cancellationToken)
    {
        var http = _httpContextAccessor.HttpContext;
        if (http?.User.Identity?.IsAuthenticated != true)
            return Result.Failure(ErrorCodes.UNAUTHORIZED);

        var sub = http.User.FindFirst("sub")?.Value
            ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return Result.Failure(ErrorCodes.TOKEN_INVALID);

        var existsUser = await _context.Users.AsNoTracking()
            .AnyAsync(u => u.UserId == userId, cancellationToken);
        if (!existsUser)
            return Result.Failure(ErrorCodes.USER_NOT_FOUND);

        var body = request.Body;
        if (body.Layout == null && body.PageSize == null && body.ShowFeatured == null && body.GridColumns == null
            && body.VisualTemplate == null)
            return Result.Failure(ErrorCodes.INVALID_DATA, "Cần ít nhất một trường cập nhật.");

        var row = await _context.UserNewsPagePreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        string merged;
        try
        {
            merged = NewsPageConfigMerge.MergePreferenceJson(row?.ValueJson, body);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ErrorCodes.INVALID_DATA, ex.Message);
        }

        var now = DateTimeOffset.UtcNow;

        if (row == null)
        {
            _context.UserNewsPagePreferences.Add(new UserNewsPagePreference
            {
                UserNewsPagePreferenceId = Guid.NewGuid(),
                UserId = userId,
                ValueJson = merged,
                UpdatedAt = now,
            });
        }
        else
        {
            row.ValueJson = merged;
            row.UpdatedAt = now;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
