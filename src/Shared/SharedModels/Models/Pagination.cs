namespace SharedModels.Models
{
    public record Pagination(bool HasNextPage, bool HasPreviousPage, long TotalItems, long PageSize, long PageNumber);
}
