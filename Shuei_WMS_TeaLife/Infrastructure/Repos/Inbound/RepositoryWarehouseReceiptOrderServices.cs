using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Inbound;
using DocumentFormat.OpenXml.Drawing;
using Domain.Entity.WMS;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System.Linq.Dynamic.Core;
using WarehouseReceiptOrder = Domain.Entity.WMS.Inbound.WarehouseReceiptOrder;
using WarehouseReceiptOrderLine = Domain.Entity.WMS.Inbound.WarehouseReceiptOrderLine;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptOrderServices : IWarehouseReceiptOrder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INumberSequences _numberSequences;
        private readonly IWarehouseTran _warehouseTranService;

        public RepositoryWarehouseReceiptOrderServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, INumberSequences numberSequences, IWarehouseTran warehouseTranService = null)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _numberSequences = numberSequences;
            _warehouseTranService = warehouseTranService;
        }

        public async Task<Result<WarehouseReceiptOrder>> AddRangeAsync([Body] List<WarehouseReceiptOrder> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == _contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo.Id;
                }

                await _dbContext.WarehouseReceiptOrders.AddRangeAsync(model);
                await _dbContext.SaveChangesAsync();
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
                _dbContext.WarehouseReceiptOrders.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
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
                _dbContext.WarehouseReceiptOrders.Remove(model);
                await _dbContext.SaveChangesAsync();
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
                return await Result<List<WarehouseReceiptOrder>>.SuccessAsync(await _dbContext.WarehouseReceiptOrders.ToListAsync());
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
                return await Result<WarehouseReceiptOrder>.SuccessAsync(await _dbContext.WarehouseReceiptOrders.FindAsync(id));
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
                await _dbContext.WarehouseReceiptOrders.AddAsync(model);
                await _dbContext.SaveChangesAsync();
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
                _dbContext.WarehouseReceiptOrders.Update(model);
                await _dbContext.SaveChangesAsync();
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
                return await Result<List<WarehouseReceiptOrder>>.SuccessAsync(await _dbContext.WarehouseReceiptOrders.Where(x => x.ReceiptNo == receiptNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> InsertWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            try
            {
                var sequenceIndex = _dbContext.SequencesNumber.Where(_ => _.JournalType == "Receipt").FirstOrDefaultAsync();

                if (sequenceIndex == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync($"ReceiptNo's prefix does not exist");
                }

                var seqResult = sequenceIndex.Result;
                var receiptIndex = $"{seqResult?.Prefix}{seqResult?.CurrentSequenceNo?.ToString().PadLeft((int)seqResult.SequenceLength, '0')}";

                if (string.IsNullOrEmpty(receiptIndex))
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync($"ReceiptNo's prefix is not valid");
                }

                var model = new WarehouseReceiptOrder
                {
                    Id = request.Id,
                    ReceiptNo = receiptIndex,
                    Location = request.Location,
                    ExpectedDate = request.ExpectedDate,
                    TenantId = request.TenantId,
                    ScheduledArrivalNumber = request.ScheduledArrivalNumber,
                    DocumentNo = request.DocumentNo,
                    SupplierId = request.SupplierId,
                    PersonInCharge = request.PersonInCharge,
                    ConfirmedBy = request.ConfirmedBy,
                    ConfirmedDate = request.ConfirmedDate,
                };

                foreach (var item in request.WarehouseReceiptOrderLines)
                {
                    item.ReceiptNo = receiptIndex;
                }

                await _dbContext.WarehouseReceiptOrders.AddAsync(model);
                seqResult.CurrentSequenceNo += 1;
                await _numberSequences.UpdateAsync(seqResult);

                var receiptOrderLines = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = receiptIndex,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    Putaway = _.Putaway,
                    UnitId = _.UnitId
                });
                await _dbContext.WarehouseReceiptOrderLines.AddRangeAsync(receiptOrderLines);

                await _dbContext.SaveChangesAsync();

                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> UpdateWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.Select(_ => new { _.Status, _.ReceiptNo }).FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);
                if (receipt.Status == EnumReceiptStatus.Open)
                {
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Ordered,
                        TransId = request.Id,
                        TransLineId = _.Id,
                        DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                        ProductCode = _.ProductCode
                    }).ToList();

                    await _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdateAsync(_ => _.SetProperty(r => r.Status, request.Status));
                    await _warehouseTranService.AddRangeAsync(warehouseTranPayload);
                    await _dbContext.SaveChangesAsync();

                    var model = await _dbContext.WarehouseReceiptOrders.FirstOrDefaultAsync(_ => _.Id == request.Id);

                    if (model == null)
                    {
                        return await Result<WarehouseReceiptOrderDto>.FailAsync("This receipt does not exist");
                    }

                    return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
                }
                else if (receipt.Status == EnumReceiptStatus.Draft)
                {
                    var model = new WarehouseReceiptOrder
                    {
                        Id = request.Id,
                        ReceiptNo = request.ReceiptNo,
                        Location = request.Location,
                        ExpectedDate = request.ExpectedDate,
                        TenantId = request.TenantId,
                        ScheduledArrivalNumber = request.ScheduledArrivalNumber,
                        DocumentNo = request.DocumentNo,
                        SupplierId = request.SupplierId,
                        PersonInCharge = request.PersonInCharge,
                        ConfirmedBy = request.ConfirmedBy,
                        ConfirmedDate = request.ConfirmedDate,
                        Status = request.Status
                    };

                    var receiptOrderLines = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseReceiptOrderLine
                    {
                        Id = _.Id,
                        ReceiptNo = model.ReceiptNo,
                        ProductCode = _.ProductCode,
                        UnitName = _.UnitName,
                        OrderQty = _.OrderQty,
                        TransQty = _.TransQty,
                        Bin = _.Bin,
                        LotNo = _.LotNo,
                        ExpirationDate = _.ExpirationDate,
                        Putaway = _.Putaway,
                        UnitId = _.UnitId
                    });

                    _dbContext.WarehouseReceiptOrders.Update(model);
                    _dbContext.WarehouseReceiptOrderLines.UpdateRange(receiptOrderLines);

                    await _dbContext.SaveChangesAsync();

                    return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
                }
                else 
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("This receipt cannot be updated as it's closed");
                }
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> GetReceiptOrderAsync(string receiptNo)
        {
            try
            {
                var receipt = await (
                from receipts in _dbContext.WarehouseReceiptOrders
                join receiptLines in (from receiptLines in _dbContext.WarehouseReceiptOrderLines.Where(r => r.ReceiptNo == receiptNo && r.IsDeleted != true)
                                    join products in _dbContext.Products.Where(p => p.IsDeleted != true) on receiptLines.ProductCode equals products.ProductCode into productReceiptLines
                                    from products in productReceiptLines.DefaultIfEmpty()
                                    select new WarehouseReceiptOrderLineDto
                                    {
                                        Id = receiptLines.Id,
                                        ReceiptNo = receiptLines.ReceiptNo,
                                        ProductCode = receiptLines.ProductCode,
                                        UnitName = receiptLines.UnitName,
                                        OrderQty = receiptLines.OrderQty,
                                        TransQty = receiptLines.TransQty,
                                        Bin = receiptLines.Bin,
                                        LotNo = receiptLines.LotNo,
                                        ExpirationDate = receiptLines.ExpirationDate,
                                        Putaway = receiptLines.Putaway,
                                        UnitId = products.UnitId,
                                        ProductName = products.ProductName,
                                        StockAvailableQuantity = products.StockAvailableQuanitty,
                                    }) 
                on receipts.ReceiptNo equals receiptLines.ReceiptNo into receiptOrderLines
                select new WarehouseReceiptOrderDto
                {
                    Id = receipts.Id,
                    ReceiptNo = receipts.ReceiptNo,
                    Location = receipts.Location,
                    ExpectedDate = receipts.ExpectedDate,
                    TenantId = receipts.TenantId,
                    ScheduledArrivalNumber = receipts.ScheduledArrivalNumber,
                    DocumentNo = receipts.DocumentNo,
                    SupplierId = receipts.SupplierId,
                    PersonInCharge = receipts.PersonInCharge,
                    ConfirmedBy = receipts.ConfirmedBy,
                    ConfirmedDate = receipts.ConfirmedDate,
                    IsDeleted = receipts.IsDeleted,
                    Status = receipts.Status,
                    WarehouseReceiptOrderLines = receiptOrderLines,
                }).FirstOrDefaultAsync(_ => _.ReceiptNo == receiptNo && _.IsDeleted != true);

                if (receipt != null)
                {
                    return await Result<WarehouseReceiptOrderDto>.SuccessAsync(receipt);
                }
                else 
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync();
                }
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrderDto>>> GetReceiptOrderListAsync()
        {
            try
            {
                var receipt = await (
                from receipts in _dbContext.WarehouseReceiptOrders.Where(_ => _.IsDeleted != true)
                join location in _dbContext.Locations.Where(_ => _.IsDeleted != true) on receipts.Location equals location.Id.ToString() into locationReceipts
                from location in locationReceipts.DefaultIfEmpty()
                join tenant in _dbContext.TenantAuth on receipts.TenantId equals tenant.TenantId into tenantReceipts
                from tenant in tenantReceipts.DefaultIfEmpty()
                join receiptLines in (from receiptLines in _dbContext.WarehouseReceiptOrderLines.Where(r => r.IsDeleted != true)
                                      join products in _dbContext.Products.Where(p => p.IsDeleted != true) on receiptLines.ProductCode equals products.ProductCode into productReceiptLines
                                      from products in productReceiptLines.DefaultIfEmpty()
                                      select new WarehouseReceiptOrderLineDto
                                      {
                                          Id = receiptLines.Id,
                                          ReceiptNo = receiptLines.ReceiptNo,
                                          ProductCode = receiptLines.ProductCode,
                                          UnitName = receiptLines.UnitName,
                                          OrderQty = receiptLines.OrderQty,
                                          TransQty = receiptLines.TransQty,
                                          Bin = receiptLines.Bin,
                                          LotNo = receiptLines.LotNo,
                                          ExpirationDate = receiptLines.ExpirationDate,
                                          Putaway = receiptLines.Putaway,
                                          UnitId = products.UnitId,
                                          ProductName = products.ProductName,
                                          StockAvailableQuantity = products.StockAvailableQuanitty,
                                      })
                on receipts.ReceiptNo equals receiptLines.ReceiptNo into receiptOrderLines
                select new WarehouseReceiptOrderDto
                {
                    Id = receipts.Id,
                    ReceiptNo = receipts.ReceiptNo,
                    Location = receipts.Location,
                    ExpectedDate = receipts.ExpectedDate,
                    TenantId = receipts.TenantId,
                    ScheduledArrivalNumber = receipts.ScheduledArrivalNumber,
                    DocumentNo = receipts.DocumentNo,
                    SupplierId = receipts.SupplierId,
                    PersonInCharge = receipts.PersonInCharge,
                    ConfirmedBy = receipts.ConfirmedBy,
                    ConfirmedDate = receipts.ConfirmedDate,
                    Status = receipts.Status,
                    LocationName = location.LocationName,
                    TenantFullName = tenant.TenantFullName,
                    WarehouseReceiptOrderLines = receiptOrderLines,
                }).ToListAsync();

                if (receipt != null)
                {
                    return await Result<List<WarehouseReceiptOrderDto>>.SuccessAsync(receipt);
                }
                else
                {
                    return await Result<List<WarehouseReceiptOrderDto>>.FailAsync();
                }
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> SyncHTData(WarehouseReceiptOrderDto receiptDto)
        {
            try
            {
                var receiptStagings = await _dbContext.WarehouseReceiptStagings.Where(_ => _.IsDeleted != true && _.ReceiptNo == receiptDto.ReceiptNo).ToListAsync();
                var receiptLineIds = receiptDto.WarehouseReceiptOrderLines.Select(line => line.Id).ToList();
                var hasValidReceiptStaging = receiptStagings
                    .Any(staging => receiptLineIds.Contains(staging.ReceiptLineId));

                if (!hasValidReceiptStaging)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("No data found for this Receipt No");
                }

                var receiptLinesDto = (
                    from receiptLine in receiptDto.WarehouseReceiptOrderLines
                    join receiptStaging in receiptStagings on receiptLine.Id equals receiptStaging.ReceiptLineId into receiptLineStaging
                    from receiptStaging in receiptLineStaging.DefaultIfEmpty()
                        select new WarehouseReceiptOrderLineDto
                        {
                            Id = receiptLine.Id,
                            ReceiptNo = receiptLine.ReceiptNo,
                            ProductCode = receiptLine.ProductCode,
                            UnitName = receiptLine.UnitName,
                            OrderQty = receiptLine.OrderQty,
                            TransQty = receiptStaging.TransQty,
                            Bin = receiptStaging.Bin,
                            LotNo = receiptStaging.LotNo,
                            ExpirationDate = receiptStaging.ExpirationDate,
                            Putaway = receiptLine.Putaway,
                            UnitId = receiptLine.UnitId,
                            ProductName = receiptLine.ProductName,
                            StockAvailableQuantity = receiptLine.StockAvailableQuantity,
                        }
                    );

                var receiptOrderLines = receiptLinesDto.Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    Putaway = _.Putaway,
                    UnitId = _.UnitId,
                });
                receiptDto.WarehouseReceiptOrderLines = receiptLinesDto;
                _dbContext.WarehouseReceiptOrderLines.UpdateRange(receiptOrderLines);
                await _dbContext.SaveChangesAsync();

                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(receiptDto);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
