using Domain.Enums;
using Application.Extentions;
using Application.Services;
using Application.Services.Base;
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
    public class RepositoryUserToTenantServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IUserToTenant
    {
        public async Task<Result<UserToTenant>> AddRangeAsync([Body] List<UserToTenant> model)
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

                await dbContext.UserToTenants.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync("Add range Tenant successfull");
            }
            catch (Exception ex)
            {
                return await Result<UserToTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserToTenant>> DeleteRangeAsync([Body] List<UserToTenant> model)
        {
            try
            {
                dbContext.UserToTenants.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync("Delete range Tenant successfull");
            }
            catch (Exception ex)
            {
                return await Result<UserToTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserToTenant>> DeleteAsync([Body] UserToTenant model)
        {
            try
            {
                dbContext.UserToTenants.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<UserToTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<UserToTenant>>> GetAllAsync()
        {
            try
            {
                return await Result<List<UserToTenant>>.SuccessAsync(await dbContext.UserToTenants.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<UserToTenant>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserToTenant>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<UserToTenant>.SuccessAsync(await dbContext.UserToTenants.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<UserToTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserToTenant>> InsertAsync([Body] UserToTenant model)
        {
            try
            {
                await dbContext.UserToTenants.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<UserToTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserToTenant>> UpdateAsync([Body] UserToTenant model)
        {
            try
            {
                dbContext.UserToTenants.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<UserToTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        async Task<Result<List<UserToTenant>>> IUserToTenant.GetByUserIdAsync([Path] string userId)
        {
            try
            {
                return await Result<List<UserToTenant>>.SuccessAsync(await dbContext.UserToTenants.Where(x => x.UserId == userId).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<UserToTenant>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
