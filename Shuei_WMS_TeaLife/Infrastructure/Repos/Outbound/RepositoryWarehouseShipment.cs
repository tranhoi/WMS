using Mapster;
using RestEase;
using Application.DTOs;
using Infrastructure.Data;
using Application.Extentions;
using Application.DTOs.Request;
using System.Linq.Dynamic.Core;
using Infrastructure.Validators;
using Microsoft.AspNetCore.Http;
using Application.Services.Outbound;
using Microsoft.EntityFrameworkCore;
using Application.Extentions.Pagings;
using Application.DTOs.Response.Product;
using MoreLinq;
using Application.Services;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehouseShipment(ApplicationDbContext dbContext, INumberSequences numberSequences, IHttpContextAccessor contextAccessor) : IWarehouseShipment
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

        public async Task<Result<WarehouseShipmentDto>> CreateWarehouseShipmentAsync([Body] WarehouseShipmentDto model)
        {
            try
            {
                //validator
                var validator = new WareHouseShipmentValidator();
                var result = validator.Validate(model);
                if (!result.IsValid)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync(String.Join(',', result.Errors));
                }
                //
                var checkRequiredShipmentNo = await dbContext.WarehouseShipments.AnyAsync(x => x.Id != model.Id && x.ShipmentNo == model.ShipmentNo);
                if (checkRequiredShipmentNo)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync("ShipmentNo is existed");
                }

                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                var warehouseShipment = model.Adapt<WarehouseShipment>();
                warehouseShipment.CreateAt = DateTime.Now;
                warehouseShipment.CreateOperatorId = userInfo == null ? "" : userInfo.Value;

                await dbContext.WarehouseShipments.AddAsync(warehouseShipment);
                await dbContext.SaveChangesAsync();
                var updateSequence = await numberSequences.IncreaseNumberSequenceByType("Shipment");

                var warehouseShipmentLines = new List<WarehouseShipmentLine>();
                if (model.WareHouseShipmentLineDtos != null && model.WareHouseShipmentLineDtos.Any())
                {
                    model.WareHouseShipmentLineDtos.ForEach(line =>
                    {
                        var item = line.Adapt<WarehouseShipmentLine>();
                        item.CreateAt = DateTime.Now;
                        item.CreateOperatorId = userInfo == null ? "" : userInfo.Value;
                        warehouseShipmentLines.Add(item);
                    });
                    await dbContext.WarehouseShipmentLines.AddRangeAsync(warehouseShipmentLines);
                }
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentDto>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        
        public async Task<Result<WarehouseShipmentDto>> UpdateWarehouseShipmentAsync([Body] WarehouseShipmentDto model)
        {
            try
            {
                var existingShipment = await dbContext.WarehouseShipments.FindAsync(model.Id);
                if (existingShipment == null)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync("Warehouse shipment not found.");
                }
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                existingShipment.UpdateAt = DateTime.Now;
                existingShipment.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;

                model.Adapt(existingShipment);
                dbContext.WarehouseShipments.Update(existingShipment);

                if (model.WareHouseShipmentLineDtos != null && model.WareHouseShipmentLineDtos.Any())
                {
                    var existingLines = await dbContext.WarehouseShipmentLines
                        .Where(l => l.ShipmentNo == model.ShipmentNo)
                        .ToListAsync();

                    dbContext.WarehouseShipmentLines.RemoveRange(existingLines);

                    var newLines = model.WareHouseShipmentLineDtos.Select(line =>
                    {
                        var item = line.Adapt<FBT.ShareModels.WMS.WarehouseShipmentLine>();
                        item.CreateAt = DateTime.Now;
                        item.CreateOperatorId = userInfo == null ? "" : userInfo.Value;
                        item.ShipmentNo = model.ShipmentNo;
                        return item;
                    }).ToList();

                    await dbContext.WarehouseShipmentLines.AddRangeAsync(newLines);
                }

                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentDto>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PageList<WarehouseShipmentDto>>> SearchWhShipments([Body] QueryModel<WarehouseShipmentSearchModel> model)
        {
            try
            {
                var data = model.Entity ?? new WarehouseShipmentSearchModel();
                var query = dbContext.WarehouseShipments.Where(x => x.IsDeleted == false
                        && (string.IsNullOrEmpty(data.ShipmentNo) || x.ShipmentNo == data.ShipmentNo)
                        && (string.IsNullOrEmpty(data.SalesNo) || x.SalesNo == data.SalesNo)
                        && (data.DeliveryDateFrom == default || x.PlanShipDate >= data.DeliveryDateFrom)
                        && (data.DeliveryDateTo == default || x.PlanShipDate <= data.DeliveryDateTo)
                        && (data.Status == default || x.Status == data.Status)
                        && (data.BinId == default || x.BinId == data.BinId.ToString())
                        && (data.TenantId == default || x.TenantId == data.TenantId)
                        && (data.LocationId == default || x.Location == data.LocationId.ToString())
                    ).OrderBy(x => x.ShipmentNo);
                var pagedList = PageList<WarehouseShipmentDto>.PagedResult(query, model.PageNumber, model.PageSize);
                return await Result<PageList<WarehouseShipmentDto>>.SuccessAsync(pagedList);
            }
            catch (Exception ex)
            {
                return await Result<PageList<WarehouseShipmentDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentDto>> GetShipmentByIdAsync([Path] Guid id)
        {
            try
            {
                var info = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.Id == id);
                if (info == null)
                    return await Result<WarehouseShipmentDto>.FailAsync($"Shipment is not existed");
                var item = await dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == info.ShipmentNo).ToListAsync();
                
                var result = new WarehouseShipmentDto(info, item);
                if (result == null)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync($"Shipment not existed");
                }

                var prodCodes = item.Select(x => x.ProductCode);
                var prodInfo = dbContext.Products.Where(_ => _.ProductStatus == EnumProductStatus.Activated && prodCodes.Contains(_.ProductCode))
                    .Join(dbContext.Units, x => x.UnitId, y => y.Id, (x, y) => new { x, y })
                    .Select(_ => new ProductDto
                    {
                        Id = _.x.Id,
                        ProductCode = _.x.ProductCode,
                        ProductName = _.x.ProductName,
                        ProductStatus = _.x.ProductStatus,
                        UnitId = _.x.UnitId,
                        UnitName = _.y.UnitName,
                        StockAvailableQuantityTrans = (int)dbContext.WarehouseTrans.Where(t => t.ProductCode == _.x.ProductCode).Sum(t => t.Qty),
                        QuantityShipment = (int)dbContext.WarehouseShipmentLines.Where(s => s.ProductCode == _.x.ProductCode
                            && s.Status == EnumShipmentOrderStatus.Open).Sum(s => s.ShipmentQty),
                    }).AsEnumerable();
                //mapping data from product
                result.WareHouseShipmentLineDtos.ForEach(item =>
                {
                    var prod = prodInfo.FirstOrDefault(x => x.ProductCode == item.ProductCode);
                    if(prod != null)
                    {
                        item.ProductName = prod.ProductName;
                        item.UnitId = prod.UnitId;
                        item.Unit = prod.UnitName;
                        item.StockAvailable = prod.StockAvailableQuantityTrans;
                        item.AvailableQuantity = prod.StockAvailableQuantityTrans - prod.QuantityShipment;
                    }
                });
                return await Result<WarehouseShipmentDto>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> ConfirmShipmentAsync([Path] Guid id)
        {
            try
            {
                var existingShipment = await dbContext.WarehouseShipments
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(existingShipment == null)
                    return await Result<bool>.FailAsync($"Shipment not existed");
                var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");
                
                existingShipment.Status = EnumShipmentOrderStatus.Open;
                existingShipment.UpdateAt = DateTime.Now;
                existingShipment.UpdateOperatorId = userInfo?.Value;
                dbContext.Update(existingShipment);
                await dbContext.SaveChangesAsync();

                var shipmentLines = dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == existingShipment.ShipmentNo).AsEnumerable();
                if(shipmentLines.Count() > 0)
                {
                    shipmentLines.ForEach(item =>
                    {
                        item.Status = EnumShipmentOrderStatus.Open;
                        item.UpdateAt = DateTime.Now;
                        item.UpdateOperatorId = userInfo?.Value;
                    });
                    dbContext.UpdateRange(shipmentLines);

                    // Add new WarehouseTrans lines
                    var warehouseTrans = shipmentLines.Select(line => new WarehouseTran
                    {
                        ProductCode = line.ProductCode,
                        Qty = -(line.ShipmentQty ?? 0),
                        Location = line.Location,
                        TenantId = existingShipment.TenantId,
                        Bin = line.Bin,
                        TransType = EnumWarehouseTransType.Shipment,
                        TransNumber = existingShipment.ShipmentNo,
                        TransId = existingShipment.Id,
                        TransLineId = line.Id,
                        StatusReceipt = null,
                        StatusIssue = EnumStatusIssue.OnOrder,
                        CreateAt = DateTime.Now,
                        CreateOperatorId = userInfo?.Value
                    }).ToList();

                    await dbContext.WarehouseTrans.AddRangeAsync(warehouseTrans);
                    await dbContext.SaveChangesAsync();
                }

                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> CreatePickingAsync(SubmitCompletedShipmentDto data)
        {
            try
            {
                var existingShipments = await dbContext.WarehouseShipments
                    .Where(x => data.Id.Contains(x.Id)).ToListAsync();

                if (existingShipments == null)
                    return await Result<bool>.FailAsync($"Shipment not existed");
                
                var shipmentNos = existingShipments.Select(existingShipment => existingShipment.ShipmentNo);
                var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");
                
                var shipmentLines = dbContext.WarehouseShipmentLines.Where(x => shipmentNos.Contains(x.ShipmentNo)).AsEnumerable();

                #region UPDATE SHIPMENT
                existingShipments.ForEach(item =>
                {
                    item.Status = EnumShipmentOrderStatus.Picking;
                    item.UpdateAt = DateTime.Now;
                    item.UpdateOperatorId = userInfo?.Value;
                    item.PickingNo = data.PickingNo;
                });
                dbContext.UpdateRange(existingShipments);
                await dbContext.SaveChangesAsync();

                if (shipmentLines.Count() > 0)
                {
                    shipmentLines.ForEach(item =>
                    {
                        item.Status = EnumShipmentOrderStatus.Picking;
                        item.UpdateAt = DateTime.Now;
                        item.UpdateOperatorId = userInfo?.Value;
                    });
                    dbContext.UpdateRange(shipmentLines);
                    await dbContext.SaveChangesAsync();
                }
                #endregion

                #region GENERATE PICKING
                var newPicking = new WarehousePickingList
                {
                    EstimatedShipDate = existingShipments.OrderByDescending(x => x.PlanShipDate).First().PlanShipDate,
                    Location = existingShipments.First().LocationName,
                    PersonInCharge = data.Manager,
                    Status = EnumShipmentOrderStatus.Picking,
                    TenantId = existingShipments.First().TenantId,
                    PickNo = data.PickingNo,
                    Remarks = data.Remarks,

                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                };
                await dbContext.AddAsync(newPicking);

                if (shipmentLines.Any())
                {
                    var pickingLines = new List<WarehousePickingLine>();
                    foreach(var group in shipmentLines.GroupBy(x => new { x.ProductCode, x.Bin }))
                    {
                        var firstItem = group.First();
                        pickingLines.Add(new WarehousePickingLine
                        {
                            ProductCode = group.Key.ProductCode,
                            Bin = group.Key.Bin,
                            Location = firstItem.Location,
                            PickNo = data.PickingNo,
                            UnitId = (int)firstItem.UnitId,
                            PickQty = 0,
                            ActualQty = group.Sum(x => x.ShipmentQty),
                            Status = EnumShipmentOrderStatus.Picking,

                            CreateAt = DateTime.Now,
                            CreateOperatorId = userInfo?.Value,
                        });
                    }
                    await dbContext.AddRangeAsync(pickingLines);
                }
                await dbContext.SaveChangesAsync();
                var updateSequence = await numberSequences.IncreaseNumberSequenceByType("Picking");
                #endregion

                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> DeleteShipmentAsync([Path] Guid id)
        {
            try
            {
                var shipment = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.Id == id);
                if (shipment == null)
                    return await Result<bool>.FailAsync($"Shipment not existed");
                dbContext.Remove(shipment);
                await dbContext.SaveChangesAsync();
                await dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == shipment.ShipmentNo)
                    .ExecuteDeleteAsync();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> CheckMultipleShipmentCreatePicking([Path] List<Guid> ids)
        {
            if(ids.Count == 0)
                return await Result<bool>.SuccessAsync(true);
            try
            {
                var shipment = dbContext.WarehouseShipments.Where(x => ids.Contains(x.Id))
                    .Select(x => new {
                        Tenant = x.TenantId,
                        ShippingCarrier = x.ShippingCarrierCode
                    }).AsEnumerable();
                if(shipment.Select(x => x.Tenant).Distinct().Count() > 1 || shipment.Select(x => x.ShippingCarrier).Distinct().Count() > 1)
                    return await Result<bool>.SuccessAsync(true);
                return await Result<bool>.SuccessAsync(false);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
