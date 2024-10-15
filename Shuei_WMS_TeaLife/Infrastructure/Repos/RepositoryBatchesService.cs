using Domain.Enums;
using Application.Extentions;
using Application.Services;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryBatchesService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IBatches
    {
        public async Task<Result<Batches>> AddRangeAsync([Body] List<Batches> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo.Id;
                    item.Status = EnumStatus.Activated;
                }

                await dbContext.Batches.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync("Add range Batch numbers successfull");
            }
            catch (Exception ex)
            {
                return await Result<Batches>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Batches>> DeleteRangeAsync([Body] List<Batches> model)
        {
            try
            {
                dbContext.Batches.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync("Delete range Batch Numbers successfull");
            }
            catch (Exception ex)
            {
                return await Result<Batches>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Batches>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Batches>>.SuccessAsync(await dbContext.Batches.ToListAsync(), $"Successfull.");
            }
            catch (Exception ex)
            {
                return await Result<List<Batches>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Batches>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Batches.FindAsync(id);
                return await Result<Batches>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<Batches>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Batches>> InsertAsync([Body] Batches model)
        {
            try
            {
                await dbContext.Batches.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync(model, $"Insert batch number sucessfull.");
            }
            catch (Exception ex)
            {
                return await Result<Batches>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<Batches>> UpdateAsync([Body] Batches model)
        {
            try
            {
                var dataUpdate = dbContext.Batches.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync(model, $"Update batch number successfull");
            }
            catch (Exception ex)
            {
                return await Result<Batches>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Batches>> DeleteAsync([Body] Batches model)
        {
            try
            {
                dbContext.Batches.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Batches>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
