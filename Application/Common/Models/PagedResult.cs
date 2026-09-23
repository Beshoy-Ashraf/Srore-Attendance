using Domain.Models;

namespace Application.Common.Models;

/// <summary>The shape every list endpoint returns.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public static PagedResult<T> From<TSource>(PagedList<TSource> source, Func<TSource, T> map, int page, int pageSize) =>
        new(source.Items.Select(map).ToList(), page, pageSize, source.TotalCount);
}
