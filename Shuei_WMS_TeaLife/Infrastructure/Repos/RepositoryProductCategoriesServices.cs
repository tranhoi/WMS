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
    public class RepositoryProducCategorysServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IProductCategory
    {
        public async Task<Result<ProductCategory>> AddRangeAsync([Body] List<ProductCategory> model)
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

                await dbContext.ProductCategories.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync("Add range ProductCategories successfull");
            }
            catch (Exception ex)
            {
                return await Result<ProductCategory>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductCategory>> DeleteRangeAsync([Body] List<ProductCategory> model)
        {
            try
            {
                dbContext.ProductCategories.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync("Delete range ProductCategories successfull");
            }
            catch (Exception ex)
            {
                return await Result<ProductCategory>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductCategory>> DeleteAsync([Body] ProductCategory model)
        {
            try
            {
                dbContext.ProductCategories.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ProductCategory>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ProductCategory>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ProductCategory>>.SuccessAsync(await dbContext.ProductCategories.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ProductCategory>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductCategory>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.ProductCategories.FindAsync(id);
                return await Result<ProductCategory>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<ProductCategory>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<ProductCategory>> InsertAsync([Body] ProductCategory model)
        {
            try
            {
                await dbContext.ProductCategories.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ProductCategory>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductCategory>> UpdateAsync([Body] ProductCategory model)
        {
            try
            {
                var dataUpdate = dbContext.ProductCategories.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ProductCategory>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
