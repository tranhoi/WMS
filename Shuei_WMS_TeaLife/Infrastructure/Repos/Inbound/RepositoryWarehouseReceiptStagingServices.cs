using Application.DTOs;
using Application.Extentions;
using Application.Services.Inbound;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Entity.Common;
using Domain.Entity.Commons;
using Domain.Entity.WMS.Inbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptStagingServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseReceiptStaging
    {
        public async Task<Result<WarehouseReceiptStaging>> AddRangeAsync([Body] List<WarehouseReceiptStaging> model)
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

                await dbContext.WarehouseReceiptStagings.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync("Add range WarehouseReceiptStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> DeleteRangeAsync([Body] List<WarehouseReceiptStaging> model)
        {
            try
            {
                dbContext.WarehouseReceiptStagings.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync("Delete range WarehouseReceiptStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> DeleteAsync([Body] WarehouseReceiptStaging model)
        {
            try
            {
                dbContext.WarehouseReceiptStagings.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptStaging>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseReceiptStaging>>.SuccessAsync(await dbContext.WarehouseReceiptStagings.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseReceiptStaging>.SuccessAsync(await dbContext.WarehouseReceiptStagings.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> InsertAsync([Body] WarehouseReceiptStaging model)
        {
            try
            {
                await dbContext.WarehouseReceiptStagings.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> UpdateAsync([Body] WarehouseReceiptStaging model)
        {
            try
            {
                dbContext.WarehouseReceiptStagings.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptStaging>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            try
            {
                return await Result<List<WarehouseReceiptStaging>>.SuccessAsync(await dbContext.WarehouseReceiptStagings.Where(x => x.ReceiptNo == receiptNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> GetByReceiptLineIdAsync([Path] Guid receiptLineId)
        {
            try
            {
                return await Result<WarehouseReceiptStaging>.SuccessAsync(await dbContext.WarehouseReceiptStagings.Where(x => x.ReceiptLineId == receiptLineId).FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
