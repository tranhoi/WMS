using Application.Extentions;
using Application.Services.Base;
using Application.Services.Inbound;
using Domain.Entity.Common;
using Domain.Entity.Commons;
using Domain.Entity.WMS.Inbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehousePutAwayStagingServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehousePutAwayStaging
    {
        public async Task<Result<WarehousePutAwayStaging>> AddRangeAsync([Body] List<WarehousePutAwayStaging> model)
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

                await dbContext.WarehousePutAwayStagings.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayStaging>.SuccessAsync("Add range WarehousePutAwayStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayStaging>> DeleteRangeAsync([Body] List<WarehousePutAwayStaging> model)
        {
            try
            {
                dbContext.WarehousePutAwayStagings.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayStaging>.SuccessAsync("Delete range WarehousePutAwayStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayStaging>> DeleteAsync([Body] WarehousePutAwayStaging model)
        {
            try
            {
                dbContext.WarehousePutAwayStagings.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAwayStaging>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePutAwayStaging>>.SuccessAsync(await dbContext.WarehousePutAwayStagings.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAwayStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayStaging>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePutAwayStaging>.SuccessAsync(await dbContext.WarehousePutAwayStagings.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayStaging>> InsertAsync([Body] WarehousePutAwayStaging model)
        {
            try
            {
                await dbContext.WarehousePutAwayStagings.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayStaging>> UpdateAsync([Body] WarehousePutAwayStaging model)
        {
            try
            {
                dbContext.WarehousePutAwayStagings.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAwayStaging>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePutAwayStaging>>.SuccessAsync(await dbContext.WarehousePutAwayStagings.Where(x => x.PutAwayNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAwayStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
