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
    public class RepositoryWarehouseReceiptOrderServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseReceiptOrder
    {
        public async Task<Result<WarehouseReceiptOrder>> AddRangeAsync([Body] List<WarehouseReceiptOrder> model)
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

                await dbContext.WarehouseReceiptOrders.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync("Add range WarehouseReceiptOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> DeleteRangeAsync([Body] List<WarehouseReceiptOrder> model)
        {
            try
            {
                dbContext.WarehouseReceiptOrders.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync("Delete range WarehouseReceiptOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> DeleteAsync([Body] WarehouseReceiptOrder model)
        {
            try
            {
                dbContext.WarehouseReceiptOrders.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrder>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseReceiptOrder>>.SuccessAsync(await dbContext.WarehouseReceiptOrders.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseReceiptOrder>.SuccessAsync(await dbContext.WarehouseReceiptOrders.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> InsertAsync([Body] WarehouseReceiptOrder model)
        {
            try
            {
                await dbContext.WarehouseReceiptOrders.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> UpdateAsync([Body] WarehouseReceiptOrder model)
        {
            try
            {
                dbContext.WarehouseReceiptOrders.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrder>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            try
            {
                return await Result<List<WarehouseReceiptOrder>>.SuccessAsync(await dbContext.WarehouseReceiptOrders.Where(x => x.ReceiptNo == receiptNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
