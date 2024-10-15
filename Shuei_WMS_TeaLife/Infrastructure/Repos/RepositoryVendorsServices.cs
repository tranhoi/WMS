﻿using Domain.Enums;
using Application.Extentions;
using Application.Services;
using Domain.Entity.Commons;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryVendorsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IVendors
    {
        public async Task<Result<Vendor>> AddRangeAsync([Body] List<Vendor> model)
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

                await dbContext.Vendors.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Vendor>.SuccessAsync("Add range Vendor successfull");
            }
            catch (Exception ex)
            {
                return await Result<Vendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Vendor>> DeleteRangeAsync([Body] List<Vendor> model)
        {
            try
            {
                dbContext.Vendors.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Vendor>.SuccessAsync("Delete range Vendor successfull");
            }
            catch (Exception ex)
            {
                return await Result<Vendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Vendor>> DeleteAsync([Body] Vendor model)
        {
            try
            {
                dbContext.Vendors.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Vendor>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Vendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Vendor>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Vendor>>.SuccessAsync(await dbContext.Vendors.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<Vendor>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Vendor>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.Vendors.FindAsync(id);
                return await Result<Vendor>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<Vendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Vendor>> InsertAsync([Body] Vendor model)
        {
            try
            {
                await dbContext.Vendors.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Vendor>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Vendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Vendor>> UpdateAsync([Body] Vendor model)
        {
            try
            {
                var dataUpdate = dbContext.Vendors.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Vendor>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Vendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
