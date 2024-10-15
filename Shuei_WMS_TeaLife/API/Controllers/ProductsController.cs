using API.Controllers.Base;
using Application.DTOs.Request.Products;
using Application.DTOs.Response.Product;
using Application.Extentions;
using Application.Services;
using Domain.Entity.Commons;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController<int, Product>, IProducts
    {
        readonly Repository _repository;

        public ProductsController(Repository repository = null!) : base(repository.SProducts)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.Product.UploadProductImage)]
        public async Task<Result<string>> UploadProductImage([Body] ProductRequestDTO model)
        {
            return await _repository.SProducts.UploadProductImage(model);
        }

        [HttpGet(ApiRoutes.Product.GetProductListAsync)]
        public async Task<Result<IEnumerable<ProductDto>>> GetProductListAsync() => await _repository.SProducts.GetProductListAsync();

        [HttpGet(ApiRoutes.Product.GetByProductCodeAsync)]
        public async Task<Result<ProductDto>> GetByProductCodeAsync(string code) => await _repository.SProducts.GetByProductCodeAsync(code);

        //[HttpGet(ApiRoutes.Product.GetByProductCodeAsync)]
        //public async Task<Result<ProductDto>> GetByProductCodeAsync([Path] string code) => await _repository.SProducts.GetByProductCodeAsync(code);
    }
}
