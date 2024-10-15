using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehousePackingLineServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) :IWarehousePackingLine
    {
        public async Task<Result<WarehousePackingLine>> AddRangeAsync([Body] List<WarehousePackingLine> model)
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

                await dbContext.WarehousePackingLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingLine>.SuccessAsync("Add range WarehousePackingLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingLine>> DeleteRangeAsync([Body] List<WarehousePackingLine> model)
        {
            try
            {
                dbContext.WarehousePackingLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingLine>.SuccessAsync("Delete range WarehousePackingLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingLine>> DeleteAsync([Body] WarehousePackingLine model)
        {
            try
            {
                dbContext.WarehousePackingLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePackingLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePackingLine>>.SuccessAsync(await dbContext.WarehousePackingLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePackingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePackingLine>.SuccessAsync(await dbContext.WarehousePackingLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingLine>> InsertAsync([Body] WarehousePackingLine model)
        {
            try
            {
                await dbContext.WarehousePackingLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePackingLine>> UpdateAsync([Body] WarehousePackingLine model)
        {
            try
            {
                dbContext.WarehousePackingLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePackingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePackingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePackingLine>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            try
            {
                return await Result<List<WarehousePackingLine>>.SuccessAsync(await dbContext.WarehousePackingLines.Where(x => x.ShipmentNo == shipmentNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePackingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
