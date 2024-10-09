using Application.Extentions;
using Application.Services.Inbound;
using Domain.Entity.WMS.Inbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptOrderLineServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseReceiptOrderLine
    {
        public async Task<Result<WarehouseReceiptOrderLine>> AddRangeAsync([Body] List<WarehouseReceiptOrderLine> model)
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

                await dbContext.WarehouseReceiptOrderLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync("Add range WarehouseReceiptOrderLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> DeleteRangeAsync([Body] List<WarehouseReceiptOrderLine> model)
        {
            try
            {
                dbContext.WarehouseReceiptOrderLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync("Delete range WarehouseReceiptOrderLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> DeleteAsync([Body] WarehouseReceiptOrderLine model)
        {
            try
            {
                dbContext.WarehouseReceiptOrderLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrderLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseReceiptOrderLine>>.SuccessAsync(await dbContext.WarehouseReceiptOrderLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(await dbContext.WarehouseReceiptOrderLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> InsertAsync([Body] WarehouseReceiptOrderLine model)
        {
            try
            {
                await dbContext.WarehouseReceiptOrderLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> UpdateAsync([Body] WarehouseReceiptOrderLine model)
        {
            try
            {
                dbContext.WarehouseReceiptOrderLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrderLine>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            try
            {
                return await Result<List<WarehouseReceiptOrderLine>>.SuccessAsync(await dbContext.WarehouseReceiptOrderLines.Where(x => x.ReceiptNo == receiptNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
