using Application.DTOs;
using Application.DTOs.Request.shipment;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Application.Models;
using Application.Services.Outbound;
using Azure;
using Dapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryPackingListServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IPackingList
    {
        public async Task<Result<string>> CompletePackingAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            try
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in model)
                        {
                            var s = await dbContext.WarehouseShipmentLines.FirstOrDefaultAsync(x => x.Id == item.Id);

                            if (s != null)
                            {
                                s.PackedQty = item.PackedQty;
                                s.PackedDate = item.PackedDate;
                                s.Status= item.StatusShipment;//   Completed = 5,
                            }
                            else
                            {
                                throw new Exception($"Shipping Line id in warehouse shipping line: {item.Id} could be not found.");
                            }

                            var whTran=await dbContext.WarehouseTrans.FirstOrDefaultAsync(x=>x.TransLineId==item.Id);
                            if(whTran != null)
                            {
                                whTran.DatePhysical=item.PackedDate;
                                whTran.StatusIssue=item.StatusIssueWhTran;// Delivered = 4,
                            }
                            else throw new Exception($"Shipping Line id in warehouse tran: {item.Id} could be not found.");
                        }

                        var shipment = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.ShipmentNo == model.FirstOrDefault().ShipmentNo);
                        if (shipment != null)
                        {
                            shipment.Status = model.FirstOrDefault().StatusShipment;
                        }
                        else throw new Exception($"Shipping No: {model.FirstOrDefault().Id} could be not found.");

                        await dbContext.SaveChangesAsync();

                        // Commit the transaction
                        await transaction.CommitAsync();

                        return await Result<string>.SuccessAsync("Update packed quantity for shipment line successfully");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        transaction.Rollback();
                        return await Result<string>.FailAsync($"Transaction update packed quantity for shipment line failed: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync($"{ex.Message} {Environment.NewLine} {ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePackingListDto>>> GetDataMasterAsync([Body] PackingListSearchRequestDto model)
        {
            try
            {
                List<WarehousePackingListDto> response = new List<WarehousePackingListDto>();

                var c = dbContext.Database.GetConnectionString();
                using (var connection = new SqlConnection(c))
                {
                    var para = new DynamicParameters();

                    para.Add("ShipmentNo", model.InstructionNumber);
                    para.Add("@PlanShipDateFrom", model.ScheduledShipDateFrom);
                    para.Add("@PlanShipDateTo", model.ScheduledShipDateTo);
                    para.Add("DeliveryLocation", model.DeliveryLocation);
                    para.Add("Bin", model.OutgoingBin);
                    para.Add("Status", model.Status);

                    var rowCount = await connection.QueryAsync<PackingListModel>("sp_packingListGetDataMaster", param: para, commandType: System.Data.CommandType.StoredProcedure);

                    var groupByShipmentNo = rowCount.GroupBy(x => x.ShipmentNo);

                    response = groupByShipmentNo.Select(x =>
                    {
                        return new WarehousePackingListDto()
                        {
                            ShipmentNo = x.Key,
                            Id = x.Select(u => u.IdShipment).FirstOrDefault(),
                            LocationName = x.Select(u => u.LocationName).FirstOrDefault(),
                            ShippingCarrierCode = x.Select(u => u.ShippingCarrierCode).FirstOrDefault(),
                            ShippingAddress = x.Select(u => u.ShippingAddress).FirstOrDefault(),
                            PlanShipDate = x.Select(u => u.PlanShipDate).FirstOrDefault(),
                            StatusOfShipment = x.Select(u => u.StatusOfShipment).FirstOrDefault(),
                            Telephone = x.Select(u => u.Telephone).FirstOrDefault(),
                            Address = x.Select(u => u.Address).FirstOrDefault(),
                            Email = x.Select(u => u.Email).FirstOrDefault(),
                            TrackingNo = x.Select(u => u.TrackingNo).FirstOrDefault(),
                            PickedDate = x.Select(u => u.PickedDate).FirstOrDefault(),
                            ShipmentLines = x.ToList(),
                        };
                    }).OrderBy(x => x.PlanShipDate).ToList();
                }

                return await Result<List<WarehousePackingListDto>>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePackingListDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<string>> UpdatePackedQtyAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            try
            {
                using (var transaction=dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in model)
                        {
                            var s = await dbContext.WarehouseShipmentLines.FirstOrDefaultAsync(x => x.Id == item.Id);

                            if (s != null)
                            {
                                s.PackedQty = item.PackedQty;
                                s.Status=item.StatusShipment; //EnumShipmentOrderStatus.Packing
                            }
                            else throw new Exception($"Shipping line Id: {item.Id} could be not found.");

                            var whTran = await dbContext.WarehouseTrans.FirstOrDefaultAsync(x => x.TransLineId == item.Id);
                            if (whTran != null)
                            {
                                whTran.StatusIssue = item.StatusIssueWhTran;//  Packing = 3,
                            }
                            else throw new Exception($"Shipping Line id in warehouse tran: {item.Id} could be not found.");
                        }

                        var shipment = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.ShipmentNo == model.FirstOrDefault().ShipmentNo);
                        if (shipment != null)
                        {
                            shipment.Status = model.FirstOrDefault().StatusShipment;//EnumShipmentOrderStatus.Packing
                        }
                        else throw new Exception($"Shipping No: {model.FirstOrDefault().Id} could be not found.");

                        await dbContext.SaveChangesAsync();

                        // Commit the transaction
                        await transaction.CommitAsync();

                        return await Result<string>.SuccessAsync("Update packed quantity for shipment line successfully");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        transaction.Rollback();
                        return await Result<string>.FailAsync($"Transaction update packed quantity for shipment line failed: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync($"{ex.Message} {Environment.NewLine} {ex.InnerException}");
            }
        }
    }
}
