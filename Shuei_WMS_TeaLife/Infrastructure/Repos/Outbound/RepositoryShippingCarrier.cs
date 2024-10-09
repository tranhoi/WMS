using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using static Application.Extentions.ApiRoutes;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryShippingCarrier(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) :IShippingCarrier
    {
        public async Task<Result<ShippingCarrier>> AddRangeAsync([Body] List<ShippingCarrier> model)
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

                await dbContext.ShippingCarriers.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync("Add range ShippingCarrier successfull");
            }
            catch (Exception ex)
            {
                return await Result<ShippingCarrier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingCarrier>> DeleteRangeAsync([Body] List<ShippingCarrier> model)
        {
            try
            {
                dbContext.ShippingCarriers.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync("Delete range ShippingCarrier successfull");
            }
            catch (Exception ex)
            {
                return await Result<ShippingCarrier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingCarrier>> DeleteAsync([Body] ShippingCarrier model)
        {
            try
            {
                dbContext.ShippingCarriers.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ShippingCarrier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ShippingCarrier>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ShippingCarrier>>.SuccessAsync(await dbContext.ShippingCarriers.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ShippingCarrier>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingCarrier>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<ShippingCarrier>.SuccessAsync(await dbContext.ShippingCarriers.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<ShippingCarrier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingCarrier>> InsertAsync([Body] ShippingCarrier model)
        {
            try
            {
                await dbContext.ShippingCarriers.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ShippingCarrier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ShippingCarrier>> UpdateAsync([Body] ShippingCarrier model)
        {
            try
            {
                dbContext.ShippingCarriers.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ShippingCarrier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
