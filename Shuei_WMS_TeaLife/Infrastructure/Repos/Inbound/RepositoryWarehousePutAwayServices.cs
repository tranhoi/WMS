using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Inbound;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehousePutAwayServices : IWarehousePutAway
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INumberSequences _numberSequences;
        private readonly DateTime now = DateTime.Now;

        public RepositoryWarehousePutAwayServices(
            ApplicationDbContext dbContext,
            IHttpContextAccessor contextAccessor,
            INumberSequences numberSequences)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _numberSequences = numberSequences;
        }

        public async Task<Result<WarehousePutAway>> AddRangeAsync([Body] List<WarehousePutAway> model)
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

                await _dbContext.WarehousePutAways.AddRangeAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync("Add range WarehousePutAway successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> DeleteRangeAsync([Body] List<WarehousePutAway> model)
        {
            try
            {
                _dbContext.WarehousePutAways.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync("Delete range WarehousePutAway successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> DeleteAsync([Body] WarehousePutAway model)
        {
            try
            {
                _dbContext.WarehousePutAways.Remove(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAway>>> GetAllAsync()
        {
            try
            {
                var result = await _dbContext.WarehousePutAways.Where(x => x.IsDeleted == false).ToListAsync();
                var locations = await _dbContext.Locations.Where(x => result.Select(xx => Guid.Parse(xx.Location)).Contains(x.Id)).ToListAsync();
                if (result.Count() > 0 && locations.Count > 0)
                {
                    result.ForEach(item =>
                    {
                        var locationInfo = locations.FirstOrDefault(x => x.Id.ToString() == item.Location);
                        if (locationInfo != null)
                            item.Location = locationInfo?.LocationName;
                    });
                }
                return await Result<List<WarehousePutAway>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAway>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePutAway>.SuccessAsync(await _dbContext.WarehousePutAways.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> InsertAsync([Body] WarehousePutAway model)
        {
            try
            {
                await _dbContext.WarehousePutAways.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> UpdateAsync([Body] WarehousePutAway model)
        {
            try
            {
                _dbContext.WarehousePutAways.Update(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAway>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePutAway>>.SuccessAsync(await _dbContext.WarehousePutAways.Where(x => x.PutAwayNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAway>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<IEnumerable<WarehousePutAwayDto>>> InsertWarehousePutAwayOrder([Body] IEnumerable<WarehousePutAwayDto> request)
        {
            try
            {
                var seqResult = await _dbContext.SequencesNumber
                    .FirstOrDefaultAsync(_ => _.JournalType == "Putaway");

                if (seqResult == null)
                    return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync("PutawayNo's prefix does not exist");

                int currentIndex = seqResult.CurrentSequenceNo ?? 6;
                var receiptNos = new List<string>();
                var warehouseReceiptOrderLines = _dbContext.WarehouseReceiptOrderLines.Where(w => request.Select(s => s.ReceiptNo).Contains(w.ReceiptNo)).ToList();
                var warehousePutAways = new List<WarehousePutAway>();
                var warehousePutAwayLines = new List<WarehousePutAwayLine>();
                var warehouseTrans = new List<WarehouseTran>();
                Dictionary<string, WarehousePutAwayDto> receiptDict = new Dictionary<string, WarehousePutAwayDto>();
                foreach (var r in request)
                {
                    var putAwayIndex = $"{seqResult.Prefix}{currentIndex.ToString().PadLeft(seqResult.SequenceLength ?? 6, '0')}";
                    currentIndex++;
                    receiptDict.Add(r.ReceiptNo, r);
                    var warehousePutAway = new WarehousePutAway
                    {
                        Id = r.Id,
                        PutAwayNo = putAwayIndex,
                        ReceiptNo = r.ReceiptNo,
                        Description = r.Description,
                        TenantId = r.TenantId,
                        
                        DocumentDate = r.DocumentDate,
                        DocumentNo = r.DocumentNo,
                        Location = r.Location,
                        PostedDate = r.PostedDate,
                        PostedBy = r.PostedBy,
                        CreateOperatorId = r.CreateOperatorId,
                        CreateAt = now,
                        IsDeleted = r.IsDeleted ?? false,
                        TransDate = DateOnly.FromDateTime(DateTime.Now)
                    };
                    receiptDict.Add(putAwayIndex, r);
                    var receiptLines = warehouseReceiptOrderLines.Where(x => x.ReceiptNo == r.ReceiptNo && x.TransQty > 0).ToList();
                    if (receiptLines.Count > 0)
                    {
                        warehousePutAways.Add(warehousePutAway);
                        foreach (var receiptLine in receiptLines)
                        {
                            warehousePutAwayLines.Add(new WarehousePutAwayLine
                            {
                                Id = Guid.NewGuid(),
                                PutAwayNo = putAwayIndex,
                                ProductCode = receiptLine.ProductCode,
                                UnitId = receiptLine.UnitId,
                                JournalQty = receiptLine.TransQty,
                                TransQty = receiptLine.TransQty,
                                ExpirationDate = receiptLine.ExpirationDate,
                                Bin = receiptLine.Bin,
                                LotNo = receiptLine.LotNo,
                                CreateAt = now,
                            });
                            var batches = _dbContext.Batches.Where(x => x.ProductCode == receiptLine.ProductCode && x.LotNo == receiptLine.LotNo).ToList();
                            if (batches.Count == 0)
                            {
                                _dbContext.Batches.Add(new Batches
                                {
                                    Id = Guid.NewGuid(),
                                    TenantId = r.TenantId,
                                    LotNo = receiptLine.LotNo,
                                    ProductCode = receiptLine.ProductCode,
                                    ExpirationDate = receiptLine.ExpirationDate,
                                    CreateAt = now,
                                    UpdateAt = now,
                                    IsDeleted = false
                                });
                            }
                            else
                            {
                                batches.ForEach(x =>
                                {
                                    x.ExpirationDate = receiptLine.ExpirationDate;
                                    x.UpdateAt = now;
                                });
                                _dbContext.Batches.UpdateRange(batches);
                            }

                        }
                    }

                }
                if (warehousePutAways.Count > 0)
                {
                    await _dbContext.WarehousePutAways.AddRangeAsync(warehousePutAways);
                    await _dbContext.WarehousePutAwayLines.AddRangeAsync(warehousePutAwayLines);

                    seqResult.CurrentSequenceNo = currentIndex;
                    _dbContext.SequencesNumber.Update(seqResult);

                    await _dbContext.SaveChangesAsync();

                    var putAwayNos = warehousePutAways.Select(_ => _.PutAwayNo).ToHashSet();
                    var warehousePutAwayLineList = await _dbContext.WarehousePutAwayLines
                        .Where(_ => putAwayNos.Contains(_.PutAwayNo))
                        .Join(_dbContext.WarehousePutAways, x => x.PutAwayNo, y => y.PutAwayNo, (x, y) => new { x, Location = y.Location })
                        .ToListAsync();

                    foreach (var line in warehousePutAwayLineList)
                    {
                        WarehousePutAwayDto r = receiptDict[line.x.PutAwayNo];
                        warehouseTrans.Add(
                        new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = line.x.PutAwayNo,
                            StatusIssue = EnumStatusIssue.OnOrder,
                            TransLineId = line.x.Id,
                            DatePhysical = null,
                            ProductCode = line.x.ProductCode,
                            Qty = (double)(-line.x.TransQty ?? 0),
                            TenantId = r.TenantId,
                            Location = r.Location,
                            Bin = line.x.Bin,
                            CreateAt = now,
                        });
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = line.x.PutAwayNo,
                            StatusReceipt = EnumStatusReceipt.Ordered,
                            TransLineId = line.x.Id,
                            DatePhysical = null,
                            ProductCode = line.x.ProductCode,
                            Qty = line.x.TransQty ?? 0,
                            TenantId = r.TenantId,
                            Location = r.Location,
                            Bin = line.x.Bin,
                            CreateAt = now,
                        }
                        );

                    }


                    await _dbContext.WarehouseTrans.AddRangeAsync(warehouseTrans);
                    await _dbContext.SaveChangesAsync();

                    return await Result<IEnumerable<WarehousePutAwayDto>>.SuccessAsync(request);
                }
                else
                {
                    return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync($"Cannot create Putaway because missing product line.");
                }
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayDto>> GetPutAwayAsync(string PutAwayNo)
        {
            /*
            try
            {
                 var putAway = await _dbContext.WarehousePutAways.FirstOrDefaultAsync(x => x.PutAwayNo == PutAwayNo);
                if (putAway == null)
                    return await Result<WarehousePutAwayDto>.FailAsync("Putaway is not existed");
                var result = putAway.Adapt<WarehousePutAwayDto>();
                result.WarehousePutAwayLines = _dbContext.WarehousePutAwayLines.Where(x => x.PutAwayNo == PutAwayNo).AsEnumerable().Adapt<List<WarehousePutAwayLineDto>>();

                return await Result<WarehousePutAwayDto>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                throw;
            }
            */
            try
            {
                var putAway = await (
                from putaways in _dbContext.WarehousePutAways
                join putawayLines in (from putawayLines in _dbContext.WarehousePutAwayLines.Where(r => r.PutAwayNo == PutAwayNo)
                                      join products in _dbContext.Products.Where(p => p.IsDeleted != true) on putawayLines.ProductCode equals products.ProductCode into productPutAwayLines
                                      from products in productPutAwayLines
                                      join units in _dbContext.Units.Where(p => p.IsDeleted != true) on putawayLines.UnitId equals units.Id into unitPutAwayLines
                                      from units in unitPutAwayLines.DefaultIfEmpty()
                                      select new WarehousePutAwayLineDto
                                      {
                                          Id = putawayLines.Id,
                                          PutAwayNo = putawayLines.PutAwayNo,
                                          ProductCode = putawayLines.ProductCode,
                                          UnitId = putawayLines.UnitId,
                                          UnitName = units!= null ? units.UnitName: "",
                                          JournalQty = putawayLines.JournalQty,
                                          TransQty = putawayLines.TransQty,
                                          Bin = putawayLines.Bin,
                                          LotNo = putawayLines.LotNo,
                                          ExpirationDate = putawayLines.ExpirationDate,
                                          ProductName = products.ProductName,
                                          ProductJanCodes = _dbContext.ProductJanCodes.Where(x => x.ProductId == products.Id).Select(x => x.JanCode).ToList(),  
                                      })
                on putaways.PutAwayNo equals putawayLines.PutAwayNo into putAwayOrderLines
                select new WarehousePutAwayDto
                {
                    Id = putaways.Id,
                    ReceiptNo = putaways.ReceiptNo,
                    Description = putaways.Description,
                    TransDate = putaways.TransDate,
                    DocumentDate = putaways.DocumentDate,
                    PutAwayNo = putaways.PutAwayNo,
                    Location = putaways.Location,
                    TenantId = putaways.TenantId,
                    DocumentNo = putaways.DocumentNo,
                    IsDeleted = putaways.IsDeleted,
                    Status = putaways.Status,
                    WarehousePutAwayLines = putAwayOrderLines.ToList(),
                }).FirstOrDefaultAsync(_ => _.PutAwayNo == PutAwayNo && _.IsDeleted != true);

                if (putAway != null)
                {
                    return await Result<WarehousePutAwayDto>.SuccessAsync(putAway);
                }
                else
                {
                    return await Result<WarehousePutAwayDto>.FailAsync();
                }
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayDto>> SyncHTData(WarehousePutAwayDto putAwayDto)
        {
            try
            {
                var putAwayStagings = await _dbContext.WarehousePutAwayStagings.Where(_ => _.IsDeleted != true && _.PutAwayNo == putAwayDto.PutAwayNo).ToListAsync();
                var putAwayLineIds = putAwayDto.WarehousePutAwayLines.Select(line => line.Id).ToList();
                var hasValidReceiptStaging = putAwayStagings
                    .Any(staging => putAwayLineIds.Contains(staging.PutAwayLineId));

                if (!hasValidReceiptStaging)
                {
                    return Result<WarehousePutAwayDto>.Fail("No data found for this Receipt No");
                }

                var putAwayLinesDto = (
                    from putAwayLine in putAwayDto.WarehousePutAwayLines
                    join putAwayStaging in putAwayStagings on putAwayLine.Id equals putAwayStaging.PutAwayLineId into putAwayLineStaging
                    from putAwayStaging in putAwayLineStaging.DefaultIfEmpty()
                    select new WarehousePutAwayLineDto(putAwayLine, putAwayStaging.JournalQty, putAwayStaging.TransQty, putAwayStaging.Bin));

                var putAwayLines = putAwayLinesDto.Select(_ => 
                new WarehousePutAwayLine(_.Id, _.PutAwayNo, _.ProductCode, _.UnitId, _.JournalQty, _.TransQty, _.Bin, _.LotNo, _.ExpirationDate,
                _.TenantId, _.Status, _.CreateAt, now));
                putAwayDto.WarehousePutAwayLines = putAwayLinesDto.ToList();
                _dbContext.WarehousePutAwayLines.UpdateRange(putAwayLines);
                await _dbContext.SaveChangesAsync();

                return Result<WarehousePutAwayDto>.Success(putAwayDto);
            }
            catch (Exception ex)
            {
                return Result<WarehousePutAwayDto>.Fail($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayDto>> AdjustActionPutAway([Body] WarehousePutAwayDto request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);

                if (receipt == null)
                {
                    return Result<WarehousePutAwayDto>.Fail($"this receipt {request.ReceiptNo} does not exist!");
                }

                var warehouseTrans = await _dbContext.WarehouseTrans.Where(_ => _.PutAwayNo == request.PutAwayNo).ToListAsync();

                foreach (var item in warehouseTrans)
                {
                    if (item.StatusReceipt == EnumStatusReceipt.Ordered) item.StatusReceipt = EnumStatusReceipt.Received;
                }
                _dbContext.WarehouseReceiptOrders.Where(_ => _.ReceiptNo == request.ReceiptNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Completed));
                _dbContext.WarehousePutAways.Where(_ => _.PutAwayNo == request.PutAwayNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumPutAwayStatus.Completed));
                _dbContext.WarehouseTrans.UpdateRange(warehouseTrans);

                await _dbContext.SaveChangesAsync();

                transaction.Commit();

                return Result<WarehousePutAwayDto>.Success(request);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result<WarehousePutAwayDto>.Fail(ex.ToString());
            }
        }
    }
}
