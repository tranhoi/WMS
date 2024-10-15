using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Authentication;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Authen

{
    [BasePath(ApiRoutes.RoleToPermissions.BasePath)]
    public interface IRoleToPermissions:IRepository<Guid,RoleToPermission>
    {
        [Get(ApiRoutes.RoleToPermissions.GetByPermissionId)]
        Task<Result<List<RoleToPermission>>> GetByPermissionsIdAsync([Path]string id);
    }
}
