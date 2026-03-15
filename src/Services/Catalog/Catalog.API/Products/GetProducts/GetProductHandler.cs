using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Marten;
using Marten.Pagination;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery(int Page = 1, int Size = 10) : IQuery<GetProductsResult>;
    public record GetProductsResult(IPagedList<Product> Products);

    public class GetProductQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            // Get product by ID from database as document using Marten
            var products = await session.Query<Product>().ToPagedListAsync(query.Page, query.Size, cancellationToken);

            return new(products);
        }
    }
}
