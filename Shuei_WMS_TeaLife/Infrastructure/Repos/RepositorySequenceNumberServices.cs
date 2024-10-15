using Domain.Enums;
using Application.Extentions;
using Application.Services;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositorySequenceNumberServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : INumberSequences
    {
        public async Task<Result<NumberSequences>> AddRangeAsync([Body] List<NumberSequences> model)
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

                await dbContext.SequencesNumber.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync("Add range Sequence numbers successfull");
            }
            catch (Exception ex)
            {
                return await Result<NumberSequences>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<NumberSequences>> DeleteRangeAsync([Body] List<NumberSequences> model)
        {
            try
            {
                dbContext.SequencesNumber.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync("Delete range Sequence Numbers successfull");
            }
            catch (Exception ex)
            {
                return await Result<NumberSequences>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<NumberSequences>>> GetAllAsync()
        {
            try
            {
                return await Result<List<NumberSequences>>.SuccessAsync(await dbContext.SequencesNumber.ToListAsync(), $"Successfull.");
            }
            catch (Exception ex)
            {
                return await Result<List<NumberSequences>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<NumberSequences>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.SequencesNumber.FindAsync(id);
                return await Result<NumberSequences>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<NumberSequences>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<NumberSequences>> InsertAsync([Body] NumberSequences model)
        {
            try
            {
                await dbContext.SequencesNumber.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync(model, $"Insert sequence number sucessfull.");
            }
            catch (Exception ex)
            {
                return await Result<NumberSequences>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<NumberSequences>> UpdateAsync([Body] NumberSequences model)
        {
            try
            {
                var dataUpdate = dbContext.SequencesNumber.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync(model, $"Update sequence number successfull");
            }
            catch (Exception ex)
            {
                return await Result<NumberSequences>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<NumberSequences>> DeleteAsync([Body] NumberSequences model)
        {
            try
            {
                dbContext.SequencesNumber.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<NumberSequences>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
