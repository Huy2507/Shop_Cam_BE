namespace Shop_Cam_BE.Application.Common.Helpers;

/// <summary>Phân trang catalog: page tối thiểu 1, pageSize có trần.</summary>
public static class StorefrontPagination
{
    /// <summary>Số bản ghi tối đa mỗi trang catalog (chống request quá lớn).</summary>
    public const int MaxCatalogPageSize = 60;

    /// <summary>Trang tối thiểu là 1.</summary>
    public static int NormalizePage(int page) => Math.Max(1, page);

    /// <summary>Giới hạn pageSize trong [1, MaxCatalogPageSize].</summary>
    public static int NormalizeCatalogPageSize(int pageSize) => Math.Clamp(pageSize, 1, MaxCatalogPageSize);
}
