using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryShippingBox(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) :IShippingBox
    {
        public async Task<Result<ShippingBox>> AddRangeAsync([Body] List<ShippingBox> model)
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

                await dbContext.ShippingBoxes.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync("Add range ShippingBox successfull");
            }
            catch (Exception ex)
            {
                return await Result<ShippingBox>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingBox>> DeleteRangeAsync([Body] List<ShippingBox> model)
        {
            try
            {
                dbContext.ShippingBoxes.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync("Delete range ShippingBox successfull");
            }
            catch (Exception ex)
            {
                return await Result<ShippingBox>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingBox>> DeleteAsync([Body] ShippingBox model)
        {
            try
            {
                dbContext.ShippingBoxes.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ShippingBox>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ShippingBox>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ShippingBox>>.SuccessAsync(await dbContext.ShippingBoxes.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ShippingBox>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingBox>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<ShippingBox>.SuccessAsync(await dbContext.ShippingBoxes.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<ShippingBox>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingBox>> InsertAsync([Body] ShippingBox model)
        {
            try
            {
                await dbContext.ShippingBoxes.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ShippingBox>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingBox>> UpdateAsync([Body] ShippingBox model)
        {
            try
            {
                dbContext.ShippingBoxes.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ShippingBox>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
