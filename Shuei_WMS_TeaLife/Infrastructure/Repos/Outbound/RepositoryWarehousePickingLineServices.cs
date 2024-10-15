using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehousePickingLineServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehousePickingLine
    {
        public async Task<Result<WarehousePickingLine>> AddRangeAsync([Body] List<WarehousePickingLine> model)
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

                await dbContext.WarehousePickingLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync("Add range WarehousePickingLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> DeleteRangeAsync([Body] List<WarehousePickingLine> model)
        {
            try
            {
                dbContext.WarehousePickingLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync("Delete range WarehousePickingLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> DeleteAsync([Body] WarehousePickingLine model)
        {
            try
            {
                dbContext.WarehousePickingLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePickingLine>>.SuccessAsync(await dbContext.WarehousePickingLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePickingLine>.SuccessAsync(await dbContext.WarehousePickingLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> InsertAsync([Body] WarehousePickingLine model)
        {
            try
            {
                await dbContext.WarehousePickingLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> UpdateAsync([Body] WarehousePickingLine model)
        {
            try
            {
                dbContext.WarehousePickingLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingLine>>> GetByMasterCodeAsync([Path] string pickingNo)
        {
            try
            {
                return await Result<List<WarehousePickingLine>>.SuccessAsync(await dbContext.WarehousePickingLines.Where(x => x.PickNo == pickingNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
