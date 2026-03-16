using SharedModels.Models;

namespace Catalog.API.Errors
{
    public static class ProductError
    {
        public static readonly Error ProductNotFound = new("404 Not Found", "Product not found");
    }
}
