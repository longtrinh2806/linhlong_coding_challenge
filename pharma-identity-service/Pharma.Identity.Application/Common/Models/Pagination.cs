namespace Pharma.Identity.Application.Common.Models;

public class Pagination<T>(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
{
    private IEnumerable<T> Data { get; set; } = data;

    private int CurrentPageNumber { get; set; } = pageNumber;

    private int PageSize { get; set; } = pageSize;

    private int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    
    private int TotalCount { get; set; } = totalCount;
}