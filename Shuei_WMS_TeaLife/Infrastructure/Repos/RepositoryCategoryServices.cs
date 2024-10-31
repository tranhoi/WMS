using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;

namespace Infrastructure.Repos
{
    public class RepositoryCategoryServices(ApplicationDbContext dbContext) : ICategories
    {
        public async Task<Result<List<SelectListItem>>> GetUserDropdown()
        {
            try
            {
                var result = await dbContext.ApplicationUsers.AsNoTracking()
                    .Select(x => new SelectListItem
                    {
                        Text = x.FullName,
                        Value = x.Id
                    }).ToListAsync();
                return await Result<List<SelectListItem>>.SuccessAsync(result);
            }                
            catch(Exception ex)
            {
                return await Result<List<SelectListItem>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<SelectListItem>>> GetBinByLocation(string? locationId = default)
        {
            try
            {
                var result = await dbContext.Bins.AsNoTracking()
                    .Where(x => x.LocationId.ToString() == locationId)
                    .Select(x => new SelectListItem
                    {
                        Text = x.BinCode,
                        Value = x.BinCode
                    }).ToListAsync();
                return await Result<List<SelectListItem>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<List<SelectListItem>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
