using Microsoft.EntityFrameworkCore;

namespace EventsWebApplication.Application.Common.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get;}
    public int TotalCount { get;}
    public int Page { get; }
    public int PageSize { get; }

    private PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> query, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var pagedResult = new PagedResult<T>(items, totalCount, page, pageSize);
        return pagedResult;
    }
    
    
}