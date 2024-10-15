using Domain.Enums;
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
    public class RepositoryLocationsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ILocations
    {
        public async Task<Result<Location>> AddRangeAsync([Body] List<Location> model)
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

                await dbContext.Locations.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync("Add range Tenant successfull");
            }
            catch (Exception ex)
            {
                return await Result<Location>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Location>> DeleteRangeAsync([Body] List<Location> model)
        {
            try
            {
                dbContext.Locations.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync("Delete range Tenant successfull");
            }
            catch (Exception ex)
            {
                return await Result<Location>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Location>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Location>>.SuccessAsync(await dbContext.Locations.ToListAsync(),$"Successfull.");
            }
            catch (Exception ex)
            {
                return await Result<List<Location>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Location>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Locations.FindAsync(id);
                return await Result<Location>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<Location>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Location>> InsertAsync([Body] Location model)
        {
            try
            {
                await dbContext.Locations.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync(model,$"Insert location {model.LocationName} sucessfull.");
            }
            catch (Exception ex)
            {
                return await Result<Location>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<List<ProductJanCode>>> GetByProductId(int productId)
        {
            try
            {
                return await Result<List<ProductJanCode>>.SuccessAsync(await dbContext.ProductJanCodes.Where(m => m.ProductId == productId).ToListAsync());

            }
            catch (Exception ex)
            {
                return await Result<List<ProductJanCode>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<Location>> UpdateAsync([Body] Location model)
        {
            try
            {
                var dataUpdate = dbContext.Locations.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync(model,$"Update location {model.LocationName} successfull");
            }
            catch (Exception ex)
            {
                return await Result<Location>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Location>> DeleteAsync([Body] Location model)
        {
            try
            {
                dbContext.Locations.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Location>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
