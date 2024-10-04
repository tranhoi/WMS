using Application.DTOs.Response.Account;
using Application.Extentions;
using Application.Models;
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
    [BasePath(ApiRoutes.Permissions.BasePath)]
    public interface IPermissions : IRepository<Guid, Permissions>
    {
        [Get(ApiRoutes.Permissions.GetAllPermissionWithAssignedRole)]
        Task<Result<List<PermissionsListResponseDTO>>> GetAllPermissionWithAssignedRoleAsync();
        [Post(ApiRoutes.Permissions.AddOrEdit)]
        Task<Result<PermissionsListResponseDTO>> AddOrEditAsync([Body] PermissionsListResponseDTO model);
    }
}
