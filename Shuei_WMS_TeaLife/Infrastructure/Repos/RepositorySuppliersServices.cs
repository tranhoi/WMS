using Application.DTOs;
using Application.Extentions;
using Application.Services.Suppliers;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositorySuppliersServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ISuppliers
    {
        public async Task<Result<Supplier>> AddRangeAsync([Body] List<Supplier> model)
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

                await dbContext.Suppliers.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync("Add range Supplier successfull");
            }
            catch (Exception ex)
            {
                return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Supplier>> DeleteRangeAsync([Body] List<Supplier> model)
        {
            try
            {
                dbContext.Suppliers.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync("Delete range Supplier successfull");
            }
            catch (Exception ex)
            {
                return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Supplier>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Supplier>>.SuccessAsync(await dbContext.Suppliers.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<Supplier>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Supplier>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.Suppliers.FindAsync(id);
                return await Result<Supplier>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Supplier>> InsertAsync([Body] Supplier model)
        {
            try
            {
                var existCD = await dbContext.Suppliers.Where(x => x.SupplierId == model.SupplierId).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    return await Result<Supplier>.FailAsync($"Suppliers Id: {model.SupplierId} is already created");
                }

                var existName = await dbContext.Suppliers.Where(x => x.SupplierName == model.SupplierName).FirstOrDefaultAsync();
                if (existName != null)
                {
                    return await Result<Supplier>.FailAsync($"Supplier name: {model.SupplierName} is already created");

                }
                await dbContext.Suppliers.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<Supplier>> UpdateAsync([Body] Supplier model)
        {
            try
            {
                var dataUpdate = dbContext.Suppliers.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Supplier>> DeleteAsync([Body] Supplier model)
        {
            try
            {
                dbContext.Suppliers.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<SupplierTenantDTO>>> GetSupplierWithTenantAsync()
        {
            try
            {
                var suppliers = await dbContext.Suppliers.ToListAsync(); // Lấy danh sách tất cả Supplier
                var supplierTenantDTOs = new List<SupplierTenantDTO>();

                foreach (var supplier in suppliers)
                {
                    // Lấy Tenant dựa trên TenantId của Supplier (nếu có TenantId)
                    var tenant = await dbContext.TenantAuth.FirstOrDefaultAsync(t => t.TenantId == supplier.CompanyId);

                    // Tạo DTO cho từng Supplier và Tenant tương ứng
                    var dto = new SupplierTenantDTO
                    {
                        Id = supplier.Id,
                        SupplierName = supplier.SupplierName,
                        SupplierId = supplier.SupplierId,
                        TenantId = supplier.CompanyId,
                        TenantFullName = tenant?.TenantFullName // Kiểm tra Tenant có null hay không
                    };

                    supplierTenantDTOs.Add(dto);
                }
                return await Result<List<SupplierTenantDTO>>.SuccessAsync(supplierTenantDTOs);
            }
            catch (Exception ex)
            {
                return await Result<List<SupplierTenantDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }           
        }
    }
}
