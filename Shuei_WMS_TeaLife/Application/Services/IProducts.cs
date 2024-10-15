using Application.DTOs.Request.Products;
using Application.DTOs.Response.Product;
using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.Commons;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.Product.BasePath)]
    public interface IProducts : IRepository<int, Product>
    {
        [Post(ApiRoutes.Product.UploadProductImage)]
        Task<Result<String>> UploadProductImage([Body] ProductRequestDTO model);


        [Get(ApiRoutes.Product.GetProductListAsync)]
        Task<Result<IEnumerable<ProductDto>>> GetProductListAsync();

        [Get(ApiRoutes.Product.GetByProductCodeAsync)]
        Task<Result<ProductDto>> GetByProductCodeAsync(string code);
    }
}
