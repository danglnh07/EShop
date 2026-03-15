using Catalog.API.Products.GetProducts;
using Mapster;

namespace Catalog.API.Profiles
{
    public class Profile
    {
        public Profile()
        {
            TypeAdapterConfig<GetProductsResult, GetProductsResponse>
                .NewConfig()
                .MapWith(src => new(src.Products,
                                    src.Products.HasNextPage,
                                    src.Products.HasPreviousPage,
                                    src.Products.TotalItemCount,
                                    src.Products.PageSize,
                                    src.Products.PageNumber));
        }
    }
}
