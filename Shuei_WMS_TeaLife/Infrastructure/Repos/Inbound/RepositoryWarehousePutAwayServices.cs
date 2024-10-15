using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Inbound;
using Domain.Entity.WMS.Inbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehousePutAwayServices : IWarehousePutAway
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INumberSequences _numberSequences;

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
                return await Result<List<WarehousePutAway>>.SuccessAsync(await _dbContext.WarehousePutAways.ToListAsync());
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
                List<WarehousePutAway> warehousePutAways = new();
                List<WarehousePutAwayLine> warehousePutAwayLines = new();

                var sequenceIndex = _dbContext.SequencesNumber.Where(_ => _.JournalType == "Putaway").FirstOrDefaultAsync();
                if (sequenceIndex == null)
                {
                    return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync($"PutawayNo's prefix does not exist");
                }

                var seqResult = sequenceIndex.Result;
                int? currentIndex = seqResult?.CurrentSequenceNo + 0;

                request.ForEach(r =>
                {
                    var putAwayIndex = $"{seqResult?.Prefix}{seqResult?.CurrentSequenceNo?.ToString().PadLeft((int)seqResult.SequenceLength, '0')}";
                    seqResult.CurrentSequenceNo += 1;

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
                        CreateAt = r.CreateAt,
                        UpdateOperatorId = r.UpdateOperatorId,
                        UpdateAt = r.UpdateAt,
                        IsDeleted = r.IsDeleted ?? false,
                        TransDate = DateOnly.FromDateTime(DateTime.Now)
                    };

                    warehousePutAways.Add(warehousePutAway);

                    if (r.WarehousePutAwayLines != null) 
                    {
                        var putAwayLines = r.WarehousePutAwayLines.Select(_ => new WarehousePutAwayLine
                        {
                            Id = _.Id,
                            PutAwayNo = _.PutAwayNo,
                            ProductCode = _.ProductCode,
                            UnitId = _.UnitId,
                            JournalQty = _.JournalQty,
                            TransQty = _.TransQty,
                            Bin = _.Bin,
                            LotNo = _.LotNo
                        }).ToList();
                        warehousePutAwayLines.AddRange(putAwayLines);
                    }
                });

                await _dbContext.WarehousePutAways.AddRangeAsync(warehousePutAways);
                await _numberSequences.UpdateAsync(seqResult);
                await _dbContext.WarehousePutAwayLines.AddRangeAsync(warehousePutAwayLines);
                await _dbContext.SaveChangesAsync();

                return await Result<IEnumerable<WarehousePutAwayDto>>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
