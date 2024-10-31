using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Outbound;

using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryInventTransferService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IInventTransfer
    {
        public async Task<Result<InventTransfer>> AddRangeAsync([Body] List<InventTransfer> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo.Id;

                    if (await CheckExist(item)) return await Result<InventTransfer>.FailAsync($"TransferNo {item.TransferNo} Is Existed");
                }

                await dbContext.InventTransfers.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync("Add range InvenTransfer successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> DeleteRangeAsync([Body] List<InventTransfer> model)
        {
            try
            {
                dbContext.InventTransfers.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync("Delete range InvenTransfer successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> DeleteAsync([Body] InventTransfer model)
        {
            try
            {
                dbContext.InventTransfers.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventTransfer>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventTransfer>>.SuccessAsync(await dbContext.InventTransfers.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventTransfer>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventTransfer>.SuccessAsync(await dbContext.InventTransfers.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> InsertAsync([Body] InventTransfer model)
        {
            try
            {
                //check required
                if (await CheckExist(model))
                    return await Result<InventTransfer>.FailAsync($"TransferNo {model.TransferNo} Is Existed");
                await dbContext.InventTransfers.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> UpdateAsync([Body] InventTransfer model)
        {
            try
            {
                //check required
                if (await dbContext.InventTransfers.FindAsync(model.Id) == null)
                    return await Result<InventTransfer>.FailAsync($"ID {model.Id} could not be found");
                dbContext.InventTransfers.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<bool> CheckExist(InventTransfer model)
        {
            return await dbContext.InventTransfers.AnyAsync(x => x.TransferNo.ToLower() == model.TransferNo.ToLower());
        }

        public async Task<Result<List<InventTransfersDTO>>> GetAllDTO()
        {
            try
            {
                var responseData = new List<InventTransfersDTO>();
                var it = await dbContext.InventTransfers.ToListAsync();

                foreach (var item in it)
                {
                    responseData.Add(new InventTransfersDTO() {
                        Id = item.Id,
                        TransferNo = item.TransferNo ,
                        IsDeleted = item.IsDeleted,
                        LocationId = item.LocationId,
                        Status = item.Status,
                        CreateAt = item.CreateAt,
                        UpdateAt = item.UpdateAt,
                        TransferDate = item.TransferDate,
                        UpdateOperatorId = item.UpdateOperatorId,
                        CreateOperatorId = item.CreateOperatorId,
                        TenantId = item.TenantId,
                        Description = item.Description,
                        PersonInCharge = item.PersonInCharge,
                        InventTransferLines = await dbContext.InventTransferLines.Where(x => x.InventTransferId == item.Id).ToListAsync(),
                    });
                }

                return await Result<List<InventTransfersDTO>>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<List<InventTransfersDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfersDTO>> GetByIdDTO(Guid id)
        {
            try
            {
                var responseData = new InventTransfersDTO();

                var it = await dbContext.InventTransfers.FindAsync(id);

                if (it == null) throw new Exception($"The Id {id} could be not found.");

                responseData.Id = it.Id;
                responseData.TransferNo = it.TransferNo;
                responseData.IsDeleted = it.IsDeleted;
                responseData.LocationId = it.LocationId;
                responseData.Status = it.Status;
                responseData.CreateAt = it.CreateAt;
                responseData.UpdateAt = it.UpdateAt;
                responseData.TransferDate = it.TransferDate;
                responseData.UpdateOperatorId = it.UpdateOperatorId;
                responseData.CreateOperatorId = it.CreateOperatorId;
                responseData.TenantId = it.TenantId;
                responseData.Description = it.Description;
                responseData.PersonInCharge = it.PersonInCharge;
                responseData.InventTransferLines = await dbContext.InventTransferLines.Where(x => x.InventTransferId == id).ToListAsync();

                return await Result<InventTransfersDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfersDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfersDTO>> GetByTransferNoDTO([Path] string transferNo)
        {
            try
            {
                var responseData = new InventTransfersDTO();
                var it = await dbContext.InventTransfers.Where(x=>x.TransferNo==transferNo).FirstOrDefaultAsync();
                if (it == null) throw new Exception($"The tranferNo {transferNo} could be not found.");

                responseData.Id = it.Id;
                responseData.TransferNo = it.TransferNo;
                responseData.IsDeleted = it.IsDeleted;
                responseData.LocationId = it.LocationId;
                responseData.Status = it.Status;
                responseData.CreateAt = it.CreateAt;
                responseData.UpdateAt = it.UpdateAt;
                responseData.TransferDate = it.TransferDate;
                responseData.UpdateOperatorId = it.UpdateOperatorId;
                responseData.CreateOperatorId = it.CreateOperatorId;
                responseData.TenantId = it.TenantId;
                responseData.Description = it.Description;
                responseData.PersonInCharge = it.PersonInCharge;
                responseData.InventTransferLines = await dbContext.InventTransferLines.Where(x => x.TransferNo == transferNo).ToListAsync();

                return await Result<InventTransfersDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfersDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
