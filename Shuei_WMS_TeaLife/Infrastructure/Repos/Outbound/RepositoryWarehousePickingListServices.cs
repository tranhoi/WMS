using Application.DTOs;
using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using Application.DTOs.Request.Picking;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

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
                // Truy vấn danh sách từ cơ sở dữ liệu
                var pickingLists = await dbContext.WarehousePickingLists.ToListAsync();

                // Kiểm tra xem danh sách có null hoặc rỗng không
                if (pickingLists == null || !pickingLists.Any())
                {
                    return await Result<List<WarehousePickingList>>.FailAsync("No data found.");
                }

                // Trả về kết quả thành công nếu có dữ liệu
                return await Result<List<WarehousePickingList>>.SuccessAsync(pickingLists);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null
                    ? $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}"
                    : ex.Message;

                return await Result<List<WarehousePickingList>>.FailAsync(errorMessage);
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
        public async Task<Result<List<WarehousePickingDTO>>> GetWarehousePickingDTOAsync([Body] PickingListSearchRequestDto model)
        {
            try
            {
                var pickings = await dbContext.WarehousePickingLists.ToListAsync();
                var pickingDTOs = new List<WarehousePickingDTO>();

                foreach (var picking in pickings)
                {
                    var planShipDates = new List<DateOnly>();
                    var shipmentNos = new List<string>();
                    var referenceDate = DateOnly.FromDateTime(DateTime.Now);

                    var warehouseShipments = await dbContext.WarehouseShipments.Where(ws => ws.PickingNo == picking.PickNo).ToListAsync();
                    if (warehouseShipments != null && warehouseShipments.Any())
                    {
                        foreach (var warehouseShipment in warehouseShipments)
                        {
                            if (warehouseShipment.PlanShipDate != null) planShipDates.Add(warehouseShipment.PlanShipDate.Value);
                            if (warehouseShipment.ShipmentNo != null) shipmentNos.Add(warehouseShipment.ShipmentNo);
                        }
                    }

                    // get the most recent planShipDate
                    DateOnly? planShipDate = new DateOnly();
                    if (planShipDates != null && planShipDates.Any())
                    {
                        planShipDate = planShipDates
                                        .OrderBy(date => Math.Abs((date.DayNumber - referenceDate.DayNumber)))
                                        .FirstOrDefault();
                    }
                    else planShipDate = null;

                    // check shipment amount
                    string shipmentNo;
                    if (shipmentNos.Count <= 0)
                    {
                        shipmentNo = "";
                    }
                    else if (shipmentNos.Count == 1)
                    {
                        shipmentNo = shipmentNos[0];
                    }
                    else
                    {
                        shipmentNo = "*";
                    }
                    var dto = new WarehousePickingDTO()
                    {
                        PickNo = picking.PickNo,
                        Location = picking.Location,
                        PersonInCharge = picking.PersonInCharge,
                        PickedDate = picking.PickedDate,
                        PlanShipDate = planShipDate,
                        Status = picking.Status,
                        ShipmentNo = shipmentNo,
                    };
                    pickingDTOs.Add(dto);
                }
                // Filter
                if (!string.IsNullOrEmpty(model.ShipmentNumber))
                {
                    pickingDTOs = pickingDTOs
                                    .Where(p => dbContext.WarehouseShipments
                                        .Any(sh => sh.PickingNo == p.PickNo && sh.ShipmentNo.Contains(model.ShipmentNumber)))
                                    .ToList();
                }

                if (!string.IsNullOrEmpty(model.Location))
                {
                    pickingDTOs = pickingDTOs.Where(p => p.Location.Contains(model.Location)).ToList();
                }

                if (!string.IsNullOrEmpty(model.Bin))
                {
                    pickingDTOs = pickingDTOs
                        .Where(p => dbContext.WarehousePickingLines
                            .Any(pl => pl.Bin == model.Bin && pl.PickNo == p.PickNo))
                        .ToList();
                }

                // Khởi tạo biến lọc
                IEnumerable<WarehousePickingDTO> filteredPickingDTOs = pickingDTOs;

                // Áp dụng điều kiện lọc cho PlanShipDateFrom
                if (model.PlanShipDateFrom != null)
                {
                    filteredPickingDTOs = filteredPickingDTOs.Where(p => p.PlanShipDate >= model.PlanShipDateFrom);
                }

                // Áp dụng điều kiện lọc cho PlanShipDateTo
                if (model.PlanShipDateTo != null)
                {
                    filteredPickingDTOs = filteredPickingDTOs.Where(p => p.PlanShipDate <= model.PlanShipDateTo);
                }

                // Lọc theo Status
                if (model.Status.HasValue)
                {
                    filteredPickingDTOs = filteredPickingDTOs.Where(p => p.Status == model.Status.Value);
                }

                // Chuyển đổi kết quả thành danh sách
                return await Result<List<WarehousePickingDTO>>.SuccessAsync(filteredPickingDTOs.ToList());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result> DeletePickingAsync([FromRoute] string pickNo)
        {
            try
            {
                List<string> errorMessages = new List<string>();
                // remove picking
                var picking = await dbContext.WarehousePickingLists
                    .FirstOrDefaultAsync(x => x.PickNo == pickNo);
                if (picking == null)
                {
                    return await Result.FailAsync("No picking found with the specified PickNo.");
                }
                dbContext.WarehousePickingLists.Remove(picking);

                //remove picking line
                var pickinglines = dbContext.WarehousePickingLines
                    .Where(p => p.PickNo == pickNo).ToList();
                if (!pickinglines.Any())
                {
                    errorMessages.Add("No picking line found with the specified PickNo.");
                }
                dbContext.WarehousePickingLines.RemoveRange(pickinglines);

                // clean picking code in shipment
                var shipments = dbContext.WarehouseShipments
                                .Where(p => p.PickingNo == pickNo).ToList();
                if (!shipments.Any())
                {
                    errorMessages.Add("No shipment found with the specified PickNo.");
                }
                else
                {
                    foreach (var shipment in shipments)
                    {
                        shipment.PickingNo = null;
                        shipment.Status = EnumShipmentOrderStatus.Open;
                        shipment.UpdateAt = DateTime.Now;
                    }
                }

                if (errorMessages.Any())
                {
                    foreach (var error in errorMessages)
                    {
                        Console.WriteLine(error);
                    }
                }

                // save changes
                await dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }
        public async Task<Result> CompletePickingAsync([FromRoute] string pickNo)
        {
            try
            {
                List<string> errorMessages = new List<string>();
                // remove picking
                var picking = await dbContext.WarehousePickingLists
                    .FirstOrDefaultAsync(x => x.PickNo == pickNo);
                if (picking == null)
                {
                    return await Result.FailAsync("No picking found with the specified PickNo.");
                }
                picking.Status = EnumShipmentOrderStatus.Picked;

                // save changes
                await dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }
        public async Task<Result<List<WarehousePickingLineDTO>>> SyncToHTAsync([Body] List<WarehousePickingLineDTO> model)
        {
            try
            {
                var pickingDTOs = model.ToList();
                foreach (var item in model)
                {
                    var pickingStaging = await dbContext.WarehousePickingStagings
                                                .FirstOrDefaultAsync(x => x.Id == item.Id);

                    if (pickingStaging == null) continue;
                    else
                    {
                        item.ActualQty = pickingStaging.ActualQty;

                        var existingDetail = pickingDTOs.FirstOrDefault(d => d.Id == item.Id);
                        if (existingDetail != null)
                        {
                            int index = pickingDTOs.IndexOf(existingDetail);
                            pickingDTOs[index] = item;
                        }
                    }                   
                }

                return await Result<List<WarehousePickingLineDTO>>.SuccessAsync(pickingDTOs);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLineDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
