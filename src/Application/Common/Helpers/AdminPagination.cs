namespace Shop_Cam_BE.Application.Common.Helpers;

/// <summary>Phân trang admin: page ≥ 1, pageSize chỉ 10 | 20 | 50 | 100.</summary>
public static class AdminPagination
{
    public const int DefaultPageSize = 20;
    public static readonly int[] AllowedPageSizes = [10, 20, 50, 100];

    public static int NormalizePage(int page) => Math.Max(1, page);

    /// <summary>Không khớp danh sách cho phép thì dùng <see cref="DefaultPageSize"/>.</summary>
    public static int NormalizePageSize(int pageSize) =>
        AllowedPageSizes.Contains(pageSize) ? pageSize : DefaultPageSize;
}
