namespace Domain.Models;

/// <summary>A single page of results plus the total number of matching rows.</summary>
public sealed record PagedList<T>(IReadOnlyList<T> Items, int TotalCount);
