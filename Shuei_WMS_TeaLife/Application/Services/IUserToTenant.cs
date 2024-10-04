using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [BasePath(ApiRoutes.UserToTenant.BasePath)]
    public interface IUserToTenant : IRepository<Guid, UserToTenant>
    {
        [Get(ApiRoutes.UserToTenant.GetByUserId)]
        Task<Result<List<UserToTenant>>> GetByUserIdAsync([Path]string userId);
    }
}