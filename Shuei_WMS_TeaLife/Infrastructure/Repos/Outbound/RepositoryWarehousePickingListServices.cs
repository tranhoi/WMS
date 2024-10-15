using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehousePickingListServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehousePickingList
    {
        public async Task<Result<WarehousePickingList>> AddRangeAsync([Body] List<WarehousePickingList> model)
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

                await dbContext.WarehousePickingLists.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync("Add range WarehousePickingList successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> DeleteRangeAsync([Body] List<WarehousePickingList> model)
        {
            try
            {
                dbContext.WarehousePickingLists.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync("Delete range WarehousePickingList successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> DeleteAsync([Body] WarehousePickingList model)
        {
            try
            {
                dbContext.WarehousePickingLists.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingList>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePickingList>>.SuccessAsync(await dbContext.WarehousePickingLists.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingList>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePickingList>.SuccessAsync(await dbContext.WarehousePickingLists.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> InsertAsync([Body] WarehousePickingList model)
        {
            try
            {
                await dbContext.WarehousePickingLists.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> UpdateAsync([Body] WarehousePickingList model)
        {
            try
            {
                dbContext.WarehousePickingLists.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingList>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePickingList>>.SuccessAsync(await dbContext.WarehousePickingLists.Where(x => x.PickNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingList>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
