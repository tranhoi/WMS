using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehouseShipment(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) :IWarehouseShipment
    {
        public async Task<Result<WarehouseShipment>> AddRangeAsync([Body] List<WarehouseShipment> model)
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

                await dbContext.WarehouseShipments.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync("Add range WarehouseShipment successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> DeleteRangeAsync([Body] List<WarehouseShipment> model)
        {
            try
            {
                dbContext.WarehouseShipments.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync("Delete range WarehouseShipment successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> DeleteAsync([Body] WarehouseShipment model)
        {
            try
            {
                dbContext.WarehouseShipments.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseShipment>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseShipment>>.SuccessAsync(await dbContext.WarehouseShipments.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseShipment>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseShipment>.SuccessAsync(await dbContext.WarehouseShipments.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> InsertAsync([Body] WarehouseShipment model)
        {
            try
            {
                await dbContext.WarehouseShipments.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> UpdateAsync([Body] WarehouseShipment model)
        {
            try
            {
                dbContext.WarehouseShipments.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseShipment>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            try
            {
                return await Result<List<WarehouseShipment>>.SuccessAsync(await dbContext.WarehouseShipments.Where(x => x.ShipmentNo == shipmentNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseShipment>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
