using Application.DTOs.Request.Products;
using Application.DTOs.Response.Product;
using Application.Enums;
using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using Product = Domain.Entity.Commons.Product;

namespace Infrastructure.Repos
{
    public class RepositoryProductsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IProducts
    {
        public async Task<Result<Product>> AddRangeAsync([Body] List<Product> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo.Id;
                }

                await dbContext.Products.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync("Add range Products successfull");
            }
            catch (Exception ex)
            {
                return await Result<Product>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Product>> DeleteRangeAsync([Body] List<Product> model)
        {
            try
            {
                dbContext.Products.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync("Delete range Products successfull");
            }
            catch (Exception ex)
            {
                return await Result<Product>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Product>> DeleteAsync([Body] Product model)
        {
            try
            {
                dbContext.Products.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Product>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Product>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Product>>.SuccessAsync(await dbContext.Products.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<Product>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Product>> GetByIdAsync([Path] int id)
        {
            try
            {
                Product result = await dbContext.Products.FindAsync(id);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.ProductImageName))
                    {
                        var fileName = @"C:\Images\Products\" + result.ProductCode + ".jpeg";
                        if (File.Exists(fileName))
                        {
                            var imageArray = File.ReadAllBytes(fileName);
                            var base64Image = Convert.ToBase64String(imageArray);
                            result.ProductImageName = base64Image;
                        }
                    }
                    return await Result<Product>.SuccessAsync(result);
                }
                else
                {
                    return await Result<Product>.FailAsync("");
                }
                    
            }
            catch (Exception ex)
            {
                return await Result<Product>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Product>> InsertAsync([Body] Product model)
        {
            try
            {
                await dbContext.Products.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Product>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Product>> UpdateAsync([Body] Product model)
        {
            try
            {
                var dataUpdate = dbContext.Products.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Product>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<String>> UploadProductImage([Path] ProductRequestDTO model)
        {
            var folderPath = @"C:\Images\Products";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var fileName = @"C:\Images\Products\" + model.ProductCode + ".jpeg";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            if (model?.ProductImage?.Length > 0)
            {
                File.WriteAllBytes(Path.Combine(folderPath, model.ProductCode + ".jpeg"), Convert.FromBase64String(model.ProductImage));
            }

            return await Result<String>.SuccessAsync(model.ProductCode + ".jpeg", "");
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetProductListAsync()
        {
            try
            {
                var result = await dbContext.Products
                .Where(product => product.IsDeleted != true)
                .Join(dbContext.ProductCategories, x => x.CategoryId, y => y.Id, (x, y) => new { x, CategoryName = y.CategoryName })
                .Join(dbContext.Units, xy => xy.x.UnitId, z => z.Id, (xy, z) => new { xy, Unitname = z.UnitName })
                .Join(dbContext.Suppliers, xyz => xyz.xy.x.SupplierId, s => s.Id, (xyz, s) => new { xyz, SupplierName = s.SupplierName})
                .Select(product => new ProductDto
                {
                    Id = product.xyz.xy.x.Id,
                    ProductCode = product.xyz.xy.x.ProductCode,
                    ProductName = product.xyz.xy.x.ProductName,
                    ProductStatus = product.xyz.xy.x.ProductStatus,
                    CategoryName = product.xyz.xy.CategoryName,
                    UnitName = product.xyz.Unitname,
                    SupplierName = product.SupplierName,
                    ProductStatusString = ((EnumProductStatus)product.xyz.xy.x.ProductStatus).ToString(),
                })
                .ToListAsync();

                return await Result<IEnumerable<ProductDto>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<ProductDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
