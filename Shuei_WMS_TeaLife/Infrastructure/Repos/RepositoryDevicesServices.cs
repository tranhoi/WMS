using Application.Extentions;
using Application.Models;
using Application.Services;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryDevicesServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IDevices
    {
        public async Task<Result<Device>> AddRangeAsync([Body] List<Device> model)
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

                await dbContext.Devices.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync("Add range Device successfull");
            }
            catch (Exception ex)
            {
                return await Result<Device>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Device>> DeleteRangeAsync([Body] List<Device> model)
        {
            try
            {
                dbContext.Devices.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync("Delete range Device successfull");
            }
            catch (Exception ex)
            {
                return await Result<Device>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Device>> DeleteAsync([Body] Device model)
        {
            try
            {
                dbContext.Devices.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Device>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Device>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Device>>.SuccessAsync(await dbContext.Devices.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<Device>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Device>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Devices.FindAsync(id);
                return await Result<Device>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<Device>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Device>> InsertAsync([Body] Device model)
        {
            try
            {
                await dbContext.Devices.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Device>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Device>> UpdateAsync([Body] Device model)
        {
            try
            {
                var dataUpdate = dbContext.Devices.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Device>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
