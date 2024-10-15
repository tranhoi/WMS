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
    public class RepositoryUnitsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IUnits
    {
        public async Task<Result<Unit>> AddRangeAsync([Body] List<Unit> model)
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

                await dbContext.Units.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Unit>.SuccessAsync("Add range Units successfull");
            }
            catch (Exception ex)
            {
                return await Result<Unit>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Unit>> DeleteRangeAsync([Body] List<Unit> model)
        {
            try
            {
                dbContext.Units.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Unit>.SuccessAsync("Delete range Units successfull");
            }
            catch (Exception ex)
            {
                return await Result<Unit>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Unit>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Unit>>.SuccessAsync(await dbContext.Units.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<Unit>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Unit>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.Units.FindAsync(id);
                return await Result<Unit>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<Unit>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Unit>> InsertAsync([Body] Unit model)
        {
            try
            {
                await dbContext.Units.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Unit>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Unit>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
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
        public async Task<Result<Unit>> UpdateAsync([Body] Unit model)
        {
            try
            {
                var dataUpdate = dbContext.Units.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Unit>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Unit>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Unit>> DeleteAsync([Body] Unit model)
        {
            try
            {
                dbContext.Units.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Unit>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Unit>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
