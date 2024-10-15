using Application.DTOs.Request.Products;
using Application.DTOs.Response.Product;
using Domain.Enums;
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
                        //var fileName = @"C:\Images\Products\" + result.ProductCode + ".jpeg";
                        var fileName = @"C:\Images\Products\" + result.ProductImageName;
                        if (File.Exists(fileName))
                        {
                            var imageArray = File.ReadAllBytes(fileName);
                            var base64Image = Convert.ToBase64String(imageArray);

                            //dồn chung ImageName và string base64 của ảnh trả về cho client cắt ra xử
                            var typeImage = result.ProductImageName.Split('.')[1];
                            if (typeImage == "png")
                            {
                                result.ProductImageName = $"{result.ProductImageName}|data:image/png;base64,{base64Image}";
                            }
                            else if (typeImage == "jpeg" || typeImage == "jpg")
                            {
                                result.ProductImageName = $"{result.ProductImageName}|data:image/jpeg;base64,{base64Image}";
                            }
                            else if (typeImage == "svg")
                            {
                                result.ProductImageName = $"{result.ProductImageName}|data:image/svg+xml;base64,{base64Image}";
                            }
                        }
                        else result.ProductImageName = string.Empty;
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
            //var fileName = @"C:\Images\Products\" + model.ProductCode + ".jpeg";
            var fileName = @"C:\Images\Products\" + model.FileNameImage;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            if (model?.ProductImage?.Length > 0)
            {
                var s = model.ProductImage.Split(',')[1];
                //File.WriteAllBytes(Path.Combine(folderPath, model.ProductCode + ".jpeg"), Convert.FromBase64String(s));
                File.WriteAllBytes(Path.Combine(folderPath, model.FileNameImage), Convert.FromBase64String(s));
            }

            //return await Result<String>.SuccessAsync(model.ProductCode + ".jpeg", "");
            return await Result<String>.SuccessAsync(model.FileNameImage, "");
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetProductListAsync()
        {
            try
            {
                var result = await dbContext.Products
                .Where(product => product.IsDeleted != true)
                .Join(dbContext.ProductCategories, x => x.CategoryId, y => y.Id, (x, y) => new { x, CategoryName = y.CategoryName })
                .Join(dbContext.Units, xy => xy.x.UnitId, z => z.Id, (xy, z) => new { xy, UnitName = z.UnitName, UnitId = z.Id })
                .Join(dbContext.Suppliers, xyz => xyz.xy.x.SupplierId, s => s.Id, (xyz, s) => new { xyz, SupplierName = s.SupplierName })
                .Select(product => new ProductDto
                {
                    Id = product.xyz.xy.x.Id,
                    ProductCode = product.xyz.xy.x.ProductCode,
                    ProductName = product.xyz.xy.x.ProductName,
                    ProductStatus = product.xyz.xy.x.ProductStatus,
                    CategoryName = product.xyz.xy.CategoryName,
                    UnitName = product.xyz.UnitName,
                    UnitId = product.xyz.UnitId,
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

        public async Task<Result<ProductDto>> GetByProductCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return await Result<ProductDto>.FailAsync($"Product code is required");
                }

                var result = await dbContext.Products.Where(_ => _.ProductCode == code)
                    .Join(dbContext.Units, x => x.UnitId, y => y.Id, (x, y) => new { x, y })
                    .Select(_ => new ProductDto
                    {
                        Id = _.x.Id,
                        ProductCode = _.x.ProductCode,
                        ProductName = _.x.ProductName,
                        ProductStatus = _.x.ProductStatus,
                        UnitName = _.y.UnitName,
                        ProductStatusString = ((EnumProductStatus)_.x.ProductStatus).ToString(),
                        StockAvailableQuantity = _.x.StockAvailableQuanitty,
                    }).FirstOrDefaultAsync();

                if (result != null)
                {
                    return await Result<ProductDto>.SuccessAsync(result);
                }
                else
                {
                    return await Result<ProductDto>.FailAsync("");
                }
            }
            catch (Exception ex)
            {
                return await Result<ProductDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
