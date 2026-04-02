using Catalog.API.Products.GetProducts;
using Mapster;

namespace Catalog.API.Profiles
{
    public class Profile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<GetProductsResult, GetProductsResponse>()
                .MapWith(src => new(src.Products, new(src.Products.HasNextPage, src.Products.HasPreviousPage, src.Products.TotalItemCount, src.Products.PageSize, src.Products.PageNumber)));
        }
    }
}
