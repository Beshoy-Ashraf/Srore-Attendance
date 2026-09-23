namespace Application.Common.Models;

public static class Paging
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 500;

    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        var size = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return (Math.Max(1, page), size);
    }
}
