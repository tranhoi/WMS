using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Inbound;
using DocumentFormat.OpenXml.Drawing;
using FBT.ShareModels.WMS;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Radzen;
using RestEase;
using System.Linq.Dynamic.Core;
using static Application.Extentions.ApiRoutes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using WarehouseReceiptOrder = FBT.ShareModels.WMS.WarehouseReceiptOrder;
using WarehouseReceiptOrderLine = FBT.ShareModels.WMS.WarehouseReceiptOrderLine;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptOrderServices : IWarehouseReceiptOrder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INumberSequences _numberSequences;
        private readonly IWarehouseTran _warehouseTranService;
        private readonly DateTime now = DateTime.Now;

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
            if (model.Status != EnumReceiptOrderStatus.Completed)
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
            else
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"This receipt cannot be deleted as its closed");
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
                    Status = EnumReceiptOrderStatus.Draft,
                    CreateAt = now,
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
                    UnitId = _.UnitId,
                    CreateAt = now,
                });
                await _dbContext.WarehouseReceiptOrderLines.AddRangeAsync(receiptOrderLines);

                await _dbContext.SaveChangesAsync();
                request.ReceiptNo = receiptIndex;

                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> UpdateWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.Select(_ => new { _.Status, _.ReceiptNo }).AsNoTracking().FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);
                if (receipt == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("This receipt does not exist");
                }

                if (receipt.Status == EnumReceiptOrderStatus.Draft)
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
                        Status = EnumReceiptOrderStatus.Draft,
                        UpdateAt = now
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
                        UnitId = _.UnitId,
                        UpdateAt = now
                    }).ToList();

                    var receiptLines = await _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo).ToListAsync();
                    var deletedReceiptLines = receiptLines.Except(receiptOrderLines, new ReceiptLineComparer());
                    var createR = receiptOrderLines.Except(receiptLines, new ReceiptLineComparer());
                    var updateR = receiptLines.Where(x => receiptOrderLines.Select(xx => xx.Id).Contains(x.Id));

                    _dbContext.WarehouseReceiptOrderLines.RemoveRange(deletedReceiptLines);
                    _dbContext.WarehouseReceiptOrders.Update(model);
                    _dbContext.WarehouseReceiptOrderLines.UpdateRange(updateR);
                    if (createR.Count() > 0) _dbContext.WarehouseReceiptOrderLines.AddRange(createR);

                    await _dbContext.SaveChangesAsync();

                    transaction.Commit();

                    return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
                }
                else 
                {
                    transaction.Rollback();
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("This receipt cannot be updated as it's Draft Status");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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
                                    from products in productReceiptLines
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
                                        ArrivalNo = receiptLines.ArrivalNo,
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
                    WarehouseReceiptOrderLines = receiptOrderLines.ToList(),
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
                join supplier in _dbContext.Suppliers on receipts.SupplierId equals supplier.Id into supplierReceipts
                from supplier in supplierReceipts.DefaultIfEmpty()
                join person in _dbContext.Users on receipts.PersonInCharge equals person.Id into personReceipts
                from person in personReceipts.DefaultIfEmpty()
                join receiptLines in (from receiptLines in _dbContext.WarehouseReceiptOrderLines.Where(r => r.IsDeleted != true)
                                      join products in _dbContext.Products.Where(p => p.IsDeleted != true) on receiptLines.ProductCode equals products.ProductCode into productReceiptLines
                                      from products in productReceiptLines
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
                    WarehouseReceiptOrderLines = receiptOrderLines.ToList(),
                    SupplierName = supplier.SupplierName,
                    PersonInChargeName = person.FullName
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
                    UpdateAt = now
                });
                receiptDto.WarehouseReceiptOrderLines = receiptLinesDto.ToList();
                _dbContext.WarehouseReceiptOrderLines.UpdateRange(receiptOrderLines);
                await _dbContext.SaveChangesAsync();

                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(receiptDto);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<WarehouseReceiptOrderDto>> CreateLineFromArrivalNo(WarehouseReceiptOrderDto receiptDto)
        {
            try
            {
                var arrivalNo = receiptDto.ScheduledArrivalNumber;

                var arrivalInstructions = await _dbContext.ArrivalInstructions.Where(_ => _.ScheduledArrivalNumber == arrivalNo).ToListAsync();
                if (arrivalInstructions.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("NoArrivalData");
                }
                var receiptLines = receiptDto.WarehouseReceiptOrderLines.Where(_ => _.ArrivalNo == arrivalNo);
                if (receiptLines.Count() > 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("ArrivalDataExist");
                }

                receiptDto.TenantId = arrivalInstructions.FirstOrDefault().CompanyId;
                receiptDto.SupplierId = arrivalInstructions.FirstOrDefault().SupplierId;
                receiptDto.ScheduledArrivalNumber = arrivalInstructions.FirstOrDefault().ScheduledArrivalNumber;
                var receiptLinesDto = (
                from arrivalInstruction in arrivalInstructions
                join product in _dbContext.Products on arrivalInstruction.ProductCode equals product.ProductCode into products
                from product in products
                join unit in _dbContext.Units on product.UnitId equals unit.Id into units
                from unit in units.DefaultIfEmpty()
                select new WarehouseReceiptOrderLineDto
                {
                    Id = Guid.NewGuid(),
                    ReceiptNo = String.IsNullOrEmpty(receiptDto.ReceiptNo) ? "" : receiptDto.ReceiptNo,
                    ProductCode = arrivalInstruction.ProductCode,
                    UnitName = unit.UnitName,
                    OrderQty = arrivalInstruction.Quantity,
                    UnitId = product.UnitId,
                    ProductName = product.ProductName,
                    StockAvailableQuantity = product.StockAvailableQuanitty,
                    ArrivalNo = arrivalNo,
                });

                var receiptOrderLines = receiptLinesDto.Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    UnitId = _.UnitId,
                    ArrivalNo = _.ArrivalNo,
                    CreateAt = now
                });


                receiptDto.WarehouseReceiptOrderLines.AddRange(receiptLinesDto.ToList());
                if (!String.IsNullOrEmpty(receiptDto.ReceiptNo))
                { 
                    _dbContext.WarehouseReceiptOrderLines.AddRange(receiptOrderLines);
                    await _dbContext.SaveChangesAsync();
                }
                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(receiptDto);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<WarehouseReceiptOrderDto>> AdjustActionReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            //using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.Select(_ => new { _.Status, _.ReceiptNo }).FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);
                if (receipt == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoReceipt");
                }
                var receiptLines = _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo).ToList();
                if (receiptLines.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProduct");
                }
                if (request.Status == EnumReceiptOrderStatus.Open)
                {
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Select(_ => new FBT.ShareModels.WMS.WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Ordered,
                        TransId = request.Id,
                        TransLineId = _.Id,
                        DatePhysical = null,
                        ProductCode = _.ProductCode,
                        Qty = _.OrderQty ?? 0,
                        TenantId = request.TenantId,
                        Location = request.Location,
                        Bin = _.Bin,
                        CreateAt = DateTime.Now,
                    }).ToList();
                    
                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Open));
                    _dbContext.WarehouseTrans.AddRange(warehouseTranPayload);
                }

                if (request.Status == EnumReceiptOrderStatus.Received)
                {
                    var orderLines = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0);
                    if (orderLines.Count() == 0)
                    {
                        
                        return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProductQty");
                    }
                    var warehouseTrans = await _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).AsNoTracking().ToListAsync();
                    foreach (var trans in warehouseTrans)
                    {
                        var receiptLine = receiptLines.Where(_ => _.Id == trans.TransLineId && _.ReceiptNo == trans.TransNumber).FirstOrDefault();
                        trans.StatusReceipt = EnumStatusReceipt.Received;
                        trans.DatePhysical = DateOnly.FromDateTime(DateTime.Now);
                        trans.Bin = receiptLine.Bin;
                        trans.LotNo = receiptLine.LotNo;
                        trans.Qty = receiptLine.TransQty ?? 0;
                        trans.UpdateAt = DateTime.Now;
                    }
                   
                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Received));
                    _dbContext.WarehouseTrans.UpdateRange(warehouseTrans);
                }

                if (request.Status == EnumReceiptOrderStatus.OnPutaway)
                {
                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.OnPutaway));
                }

                await _dbContext.SaveChangesAsync();

                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync(ex.ToString());
            }
        }
    }

    public class ReceiptLineComparer : IEqualityComparer<WarehouseReceiptOrderLine>
    {
        public bool Equals(WarehouseReceiptOrderLine x, WarehouseReceiptOrderLine y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(WarehouseReceiptOrderLine obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
