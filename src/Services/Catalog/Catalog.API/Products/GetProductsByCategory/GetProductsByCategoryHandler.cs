
using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Marten;
using Marten.Pagination;

namespace Catalog.API.Products.GetProductsByCategory
{
    public record GetProductsByCategoryQuery(string Category, int Page = 1, int Size = 10) : IQuery<GetProductsByCategoryResult>;
    public record GetProductsByCategoryResult(IPagedList<Product> Products);

    public class GetProductsByCategoryHandler(IDocumentSession session) : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {
        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>()
                .Where(p => p.Categories.Contains(query.Category))
                .ToPagedListAsync(query.Page, query.Size, cancellationToken);

            return new(products);
        }
    }
}
