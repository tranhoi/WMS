﻿using Application.DTOs;
using Domain.Enums;
using Application.Extentions;
using Application.Models;
using Application.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Radzen;
using RestEase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryBinsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IBins
    {
        public async Task<Result<List<Bin>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Bin>>.SuccessAsync(await dbContext.Bins.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<Bin>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Bin>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Bins.FindAsync(id);
                return await Result<Bin>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<Bin>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Bin>> InsertAsync([Body] Bin model)
        {
            try
            {
                await dbContext.Bins.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Bin>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Bin>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Bin>> UpdateAsync([Body] Bin model)
        {
            try
            {
                var dataUpdate = dbContext.Bins.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Bin>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Bin>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Bin>>> GetByLocationId([Path] Guid locationId)
        {
            try
            {
                return await Result<List<Bin>>.SuccessAsync(await dbContext.Bins.AsNoTracking().Where(m => m.LocationId == locationId).ToListAsync());

            }
            catch (Exception ex)
            {
                return await Result<List<Bin>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Bin>> DeleteAsync([Body] Bin model)
        {
            try
            {
                dbContext.Bins.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Bin>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Bin>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Bin>> AddRangeAsync([Body] List<Bin> model)
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

                await dbContext.Bins.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Bin>.SuccessAsync("Add range Bins successfull");
            }
            catch (Exception ex)
            {
                return await Result<Bin>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Bin>> DeleteRangeAsync([Body] List<Bin> model)
        {
            try
            {
                dbContext.Bins.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Bin>.SuccessAsync("Delete range Bins successfull");
            }
            catch (Exception ex)
            {
                return await Result<Bin>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Bin>>> AddOrUpdateAsync([Body] List<Bin> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    var check = await dbContext.Bins.FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (check != null)
                    {
                        //dbContext.Entry(check).CurrentValues.SetValues(item);

                        check.UpdateAt = DateTime.Now;
                        check.UpdateOperatorId = userInfo.Id;
                        check.BinCode = item.BinCode;
                        check.Remarks = item.Remarks;

                        //dbContext.Bins.Update(item);
                    }
                    else
                    {
                        item.CreateAt = DateTime.Now;
                        item.CreateOperatorId = userInfo.Id;

                        var responseBin = await dbContext.Bins.AddAsync(item);
                    }
                }

                await dbContext.SaveChangesAsync();

                return await Result<List<Bin>>.SuccessAsync(model, $"Successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<Bin>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] string id)
        {
            try
            {
                var dataInfo = await dbContext.Bins.FindAsync(id);
                if (dataInfo == null) return null;

                List<LabelInfoDto> res = new List<LabelInfoDto>();

                res.Add(new LabelInfoDto()
                {
                    Title="BIN",
                    QrValue = GlobalVariable.GenerateQRCode($"{dataInfo.LocationCD}|{dataInfo.LocationName}|{dataInfo.BinCode}"),
                    Title1 = "Location Name:",
                    Content1 = dataInfo.LocationName,
                    Title2 = "BIN Code:",
                    Content2 = dataInfo.BinCode,
                });

                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<LabelInfoDto>> GetLabelByLocationIdAsync([Path] Guid locationId)
        {
            try
            {
                var dataInfo = await dbContext.Bins.Where(m => m.LocationId == locationId).ToListAsync();
                if (dataInfo == null) return null;

                List<LabelInfoDto> res = new List<LabelInfoDto>();

                foreach (var item in dataInfo)
                {
                    res.Add(new LabelInfoDto()
                    {
                        Title = "BIN",
                        QrValue = GlobalVariable.GenerateQRCode($"{item.LocationCD}|{item.LocationName}|{item.BinCode}"),
                        Title1 = "Location Name:",
                        Content1 = item.LocationName,
                        Title2 = "BIN Code:",
                        Content2 = item.BinCode,
                    });
                }

                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
