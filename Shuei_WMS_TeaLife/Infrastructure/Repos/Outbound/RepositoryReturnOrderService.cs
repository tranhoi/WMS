using Application.DTOs;
using Application.Extentions;
using Application.Services.Outbound;
using Infrastructure.Data;
using Infrastructure.Validators;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    class RepositoryReturnOrderService : IReturnOrder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public RepositoryReturnOrderService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<ReturnOrder>> AddRangeAsync([Body] List<ReturnOrder> model)
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

                await _dbContext.ReturnOrders.AddRangeAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync("Add range ReturnOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> DeleteAsync([Body] ReturnOrder model)
        {
            try
            {
                _dbContext.ReturnOrders.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync("Delete range ReturnOrders successfull");
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> DeleteRangeAsync([Body] List<ReturnOrder> model)
        {
            try
            {
                _dbContext.ReturnOrders.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync("Delete range ReturnOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ReturnOrder>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ReturnOrder>>.SuccessAsync(await _dbContext.ReturnOrders.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ReturnOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<ReturnOrder>.SuccessAsync(await _dbContext.ReturnOrders.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> InsertAsync([Body] ReturnOrder model)
        {
            try
            {
                await _dbContext.ReturnOrders.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> UpdateAsync([Body] ReturnOrder model)
        {
            try
            {
                _dbContext.ReturnOrders.Update(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ReturnOrderDto>>> GetAllReturnOrdersAsync()
        {
            try
            {
                var result =( from returnOrder in _dbContext.ReturnOrders.Where(_ => _.IsDeleted != true)
                    join returnOrderLines in _dbContext.ReturnOrderLines.Where(_ => _.IsDeleted != true) on returnOrder.ReturnOrderNo equals returnOrderLines.ReturnOrderNo into returnOrders
                    from returnOrderLines in returnOrders
                    select new ReturnOrderDto(returnOrder, returnOrders.ToList())
                );

                return await Result<List<ReturnOrderDto>>.SuccessAsync(await result.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ReturnOrderDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrderDto>> GetReturnOrderByReturnNoAsync(string returnOrderNo)
        {
            try
            {
                var result = (from returnOrder in _dbContext.ReturnOrders.Where(_ => _.IsDeleted != true && _.ReturnOrderNo == returnOrderNo)
                              join returnOrderLine in _dbContext.ReturnOrderLines.Where(_ => _.IsDeleted != true) on returnOrder.ReturnOrderNo equals returnOrderLine.ReturnOrderNo into returnOrderLines
                              from returnOrderLine in returnOrderLines
                              select new ReturnOrderDto(returnOrder, returnOrderLines.ToList())
                ).FirstOrDefaultAsync();

                return await Result<ReturnOrderDto>.SuccessAsync(await result);
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrderDto>> InsertReturnOrderAsync([Body] ReturnOrderDto dto)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var validator = new ReturnOrderValidator();
                var result = validator.Validate(dto);
                if (!result.IsValid)
                {
                    return Result<ReturnOrderDto>.Fail(string.Join(',', result.Errors));
                }

                bool isReturnOrderExisted = await _dbContext.ReturnOrders.AnyAsync(_ => _.ReturnOrderNo == dto.ReturnOrderNo);
                if (isReturnOrderExisted)
                {
                    return Result<ReturnOrderDto>.Fail("This ReturnOrderDto have already existed");
                }

                var userInfo = _contextAccessor?.HttpContext?.User.FindFirst("UserId");
                var returnOrder = dto.Adapt<ReturnOrder>();
                returnOrder.CreateAt = DateTime.Now;
                returnOrder.CreateOperatorId = userInfo?.Value;
                await _dbContext.ReturnOrders.AddAsync(returnOrder);

                if (dto.ReturnOrderLines.Any())
                {
                    var returnOrderLines = dto.ReturnOrderLines.Adapt<List<ReturnOrderLine>>();
                    foreach (var returnOrderLine in returnOrderLines)
                    {
                        returnOrderLine.CreateAt = DateTime.Now;
                        returnOrderLine.CreateOperatorId = userInfo?.Value;
                    }
                    await _dbContext.ReturnOrderLines.AddRangeAsync(returnOrderLines);
                }

                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                return await Result<ReturnOrderDto>.SuccessAsync(dto);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<ReturnOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrderDto>> UpdateReturnOrderAsync([Body] ReturnOrderDto dto)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var validator = new ReturnOrderValidator();
                var result = validator.Validate(dto);
                if (!result.IsValid)
                {
                    return Result<ReturnOrderDto>.Fail(string.Join(',', result.Errors));
                }

                var userInfo = _contextAccessor?.HttpContext?.User.FindFirst("UserId");
                var returnOrder = dto.Adapt<ReturnOrder>();
                returnOrder.UpdateAt = DateTime.Now;
                returnOrder.UpdateOperatorId = userInfo?.Value;
                _dbContext.ReturnOrders.Update(returnOrder);

                if (dto.ReturnOrderLines.Any())
                {
                    var returnOrderLines = dto.ReturnOrderLines.Adapt<List<ReturnOrderLine>>();
                    foreach (var returnOrderLine in returnOrderLines)
                    {
                        returnOrderLine.UpdateAt = DateTime.Now;
                        returnOrderLine.UpdateOperatorId = userInfo?.Value;
                    }
                    _dbContext.ReturnOrderLines.UpdateRange(returnOrderLines);
                }

                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                return await Result<ReturnOrderDto>.SuccessAsync(dto);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<ReturnOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> DeleteReturnOrderAsync([Path] Guid id)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var returnOrder = await _dbContext.ReturnOrders.FirstOrDefaultAsync(_ => _.Id == id);
                if (returnOrder == null) return Result<bool>.Fail("This ReturnOrderDto have already existed");

                _dbContext.ReturnOrders.Remove(returnOrder);
                await _dbContext.ReturnOrders.Where(_ => _.ReturnOrderNo == returnOrder.ReturnOrderNo).ExecuteDeleteAsync();
                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
