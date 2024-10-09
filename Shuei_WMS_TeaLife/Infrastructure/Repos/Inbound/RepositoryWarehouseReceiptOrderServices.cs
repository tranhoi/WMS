using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Inbound;
using DocumentFormat.OpenXml.Drawing;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using WarehouseReceiptOrder = Domain.Entity.WMS.Inbound.WarehouseReceiptOrder;
using WarehouseReceiptOrderLine = Domain.Entity.WMS.Inbound.WarehouseReceiptOrderLine;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptOrderServices : IWarehouseReceiptOrder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INumberSequences _numberSequences;

        public RepositoryWarehouseReceiptOrderServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, INumberSequences numberSequences)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _numberSequences = numberSequences;
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

        public async Task<Result<WarehouseReceiptOrder>> InsertWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            try
            {
                var sequenceIndex = _dbContext.SequencesNumber.Where(_ => _.JournalType == "Receipt").FirstOrDefaultAsync();

                if (sequenceIndex == null)
                {
                    return await Result<WarehouseReceiptOrder>.FailAsync($"ReceiptNo's prefix does not exist");
                }

                var seqResult = sequenceIndex.Result;
                var receiptIndex = $"{seqResult?.Prefix}{seqResult?.CurrentSequenceNo?.ToString().PadLeft((int)seqResult.SequenceLength, '0')}";

                if (string.IsNullOrEmpty(receiptIndex))
                {
                    return await Result<WarehouseReceiptOrder>.FailAsync($"ReceiptNo's prefix is not valid");
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
                    CreateOperatorId = request.CreateOperatorId,
                    CreateAt = request.CreateAt,
                    UpdateOperatorId = request.UpdateOperatorId,
                    UpdateAt = request.UpdateAt,
                    IsDeleted = request.IsDeleted ?? false,
                    Status = request.Status
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

                return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> UpdateWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            try
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
                    CreateOperatorId = request.CreateOperatorId,
                    CreateAt = request.CreateAt,
                    UpdateOperatorId = request.UpdateOperatorId,
                    UpdateAt = request.UpdateAt,
                    IsDeleted = request.IsDeleted ?? false,
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

                return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
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
                                        UnitId = receiptLines.UnitId,
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
                    CreateOperatorId = receipts.CreateOperatorId,
                    CreateAt = receipts.CreateAt,
                    UpdateOperatorId = receipts.UpdateOperatorId,
                    UpdateAt = receipts.UpdateAt,
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
                    CreateOperatorId = receipts.CreateOperatorId,
                    CreateAt = receipts.CreateAt,
                    UpdateOperatorId = receipts.UpdateOperatorId,
                    UpdateAt = receipts.UpdateAt,
                    IsDeleted = receipts.IsDeleted,
                    Status = receipts.Status,
                    LocationName = location.LocationName,
                    TenantFullName = tenant.TenantFullName,
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
    }
}
