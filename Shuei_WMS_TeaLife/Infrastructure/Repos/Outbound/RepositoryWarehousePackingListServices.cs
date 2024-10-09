using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehousePackingListServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) :IWarehousePackingList
    {
        public async Task<Result<WarehousePackingList>> AddRangeAsync([Body] List<WarehousePackingList> model)
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

                await dbContext.WarehousePackingLists.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingList>.SuccessAsync("Add range WarehousePackingList successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingList>> DeleteRangeAsync([Body] List<WarehousePackingList> model)
        {
            try
            {
                dbContext.WarehousePackingLists.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingList>.SuccessAsync("Delete range WarehousePackingList successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingList>> DeleteAsync([Body] WarehousePackingList model)
        {
            try
            {
                dbContext.WarehousePackingLists.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePackingList>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePackingList>>.SuccessAsync(await dbContext.WarehousePackingLists.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePackingList>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingList>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePackingList>.SuccessAsync(await dbContext.WarehousePackingLists.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingList>> InsertAsync([Body] WarehousePackingList model)
        {
            try
            {
                await dbContext.WarehousePackingLists.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingList>> UpdateAsync([Body] WarehousePackingList model)
        {
            try
            {
                dbContext.WarehousePackingLists.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePackingList>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            try
            {
                return await Result<List<WarehousePackingList>>.SuccessAsync(await dbContext.WarehousePackingLists.Where(x => x.ShipmentNo == shipmentNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePackingList>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
