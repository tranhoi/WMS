using Domain.Enums;
using Application.Extentions;
using Application.Services.Inbound;
using Domain.Entity.Commons;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Extentions.ApiRoutes;

namespace Infrastructure.Repos
{
    public class RepositoryArrivalInstructionServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IArrivalInstructions
    {
        public async Task<Result<ArrivalInstruction>> AddRangeAsync([Body] List<ArrivalInstruction> model)
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

                await dbContext.ArrivalInstructions.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync("Add range ArrivalInstructions successfull");
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> DeleteRangeAsync([Body] List<ArrivalInstruction> model)
        {
            try
            {
                dbContext.ArrivalInstructions.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync("Delete range ArrivalInstructions successfull");
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> DeleteAsync([Body] ArrivalInstruction model)
        {
            try
            {
                dbContext.ArrivalInstructions.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ArrivalInstruction>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ArrivalInstruction>>.SuccessAsync(await dbContext.ArrivalInstructions.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ArrivalInstruction>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> GetByIdAsync([Path] int id)
        {
            try
            {
                return await Result<ArrivalInstruction>.SuccessAsync(await dbContext.ArrivalInstructions.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> InsertAsync([Body] ArrivalInstruction model)
        {
            try
            {
                await dbContext.ArrivalInstructions.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> UpdateAsync([Body] ArrivalInstruction model)
        {
            try
            {
                dbContext.ArrivalInstructions.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
