using Application.Extentions;
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
    public class RepositoryWarehousePutAwayServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehousePutAway
    {
        public async Task<Result<WarehousePutAway>> AddRangeAsync([Body] List<WarehousePutAway> model)
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

                await dbContext.WarehousePutAways.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync("Add range WarehousePutAway successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> DeleteRangeAsync([Body] List<WarehousePutAway> model)
        {
            try
            {
                dbContext.WarehousePutAways.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync("Delete range WarehousePutAway successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> DeleteAsync([Body] WarehousePutAway model)
        {
            try
            {
                dbContext.WarehousePutAways.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAway>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePutAway>>.SuccessAsync(await dbContext.WarehousePutAways.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAway>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePutAway>.SuccessAsync(await dbContext.WarehousePutAways.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> InsertAsync([Body] WarehousePutAway model)
        {
            try
            {
                await dbContext.WarehousePutAways.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> UpdateAsync([Body] WarehousePutAway model)
        {
            try
            {
                dbContext.WarehousePutAways.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAway>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePutAway>>.SuccessAsync(await dbContext.WarehousePutAways.Where(x => x.PutAwayNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAway>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
