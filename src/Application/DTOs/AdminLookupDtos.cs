namespace Shop_Cam_BE.Application.DTOs;

public class AdminCategoryLookupDto
{
    public Guid ProductCategoryId { get; set; }
    public string Name { get; set; } = default!;
}
