using Domain.Enums;
using Application.Extentions;
using Application.Services;
using Domain.Entity.Commons;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryProductJanCodesServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IProductJanCodes
    {
        public async Task<Result<ProductJanCode>> AddRangeAsync([Body] List<ProductJanCode> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo.Id;
                    item.Status = EnumStatus.Activated;

                }

                await dbContext.ProductJanCodes.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync("Add range ProductJanCodes successfull");
            }
            catch (Exception ex)
            {
                return await Result<ProductJanCode>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductJanCode>> DeleteRangeAsync([Body] List<ProductJanCode> model)
        {
            try
            {
                dbContext.ProductJanCodes.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync("Delete range ProductJanCodes successfull");
            }
            catch (Exception ex)
            {
                return await Result<ProductJanCode>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ProductJanCode>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ProductJanCode>>.SuccessAsync(await dbContext.ProductJanCodes.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ProductJanCode>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductJanCode>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.ProductJanCodes.FindAsync(id);
                return await Result<ProductJanCode>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<ProductJanCode>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductJanCode>> InsertAsync([Body] ProductJanCode model)
        {
            try
            {
                await dbContext.ProductJanCodes.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ProductJanCode>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductJanCode>> UpdateAsync([Body] ProductJanCode model)
        {
            try
            {
                var dataUpdate = dbContext.ProductJanCodes.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ProductJanCode>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<List<ProductJanCode>>> GetByProductId([Path] int productId)
        {
            try
            {
                return await Result<List<ProductJanCode>>.SuccessAsync(await dbContext.ProductJanCodes.Where(m => m.ProductId == productId).ToListAsync());

            }
            catch (Exception ex)
            {
                return await Result<List<ProductJanCode>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductJanCode>> DeleteAsync([Body] ProductJanCode model)
        {
            try
            {
                dbContext.ProductJanCodes.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ProductJanCode>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ProductJanCode>>> AddOrUpdateAsync([Body] List<ProductJanCode> model)
        {
            try
            {
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                dbContext.ProductJanCodes.UpdateRange(model);
                await dbContext.SaveChangesAsync();

                return await Result<List<ProductJanCode>>.SuccessAsync(model, $"Successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<ProductJanCode>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
