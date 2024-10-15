using Application.Extentions;
using Application.Services;
using Application.Services.Inbound;
using Domain.Entity.Common;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Domain.Entity.WMS.Inbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseTranServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseTran
    {
        public async Task<Result<WarehouseTran>> AddRangeAsync([Body] List<WarehouseTran> model)
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

                await dbContext.WarehouseTrans.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync("Add range WarehouseTran successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseTran>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseTran>> DeleteRangeAsync([Body] List<WarehouseTran> model)
        {
            try
            {
                dbContext.WarehouseTrans.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync("Delete range WarehouseTran successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseTran>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseTran>> DeleteAsync([Body] WarehouseTran model)
        {
            try
            {
                dbContext.WarehouseTrans.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseTran>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseTran>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseTran>>.SuccessAsync(await dbContext.WarehouseTrans.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseTran>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseTran>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseTran>.SuccessAsync(await dbContext.WarehouseTrans.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseTran>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseTran>> InsertAsync([Body] WarehouseTran model)
        {
            try
            {
                await dbContext.WarehouseTrans.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseTran>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseTran>> UpdateAsync([Body] WarehouseTran model)
        {
            try
            {
                dbContext.WarehouseTrans.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseTran>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
