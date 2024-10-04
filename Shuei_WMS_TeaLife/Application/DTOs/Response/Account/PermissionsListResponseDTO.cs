using Domain.Entity.WMS.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.Account
{
    public class PermissionsListResponseDTO : Permissions
    {
        public List<GetRoleResponseDTO> AssignedToRoles { get; set; } = new List<GetRoleResponseDTO>();
    }
}
