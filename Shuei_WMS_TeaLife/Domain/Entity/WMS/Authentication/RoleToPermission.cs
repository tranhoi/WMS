using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS.Authentication
{
    public class RoleToPermission : GenericEntity
    {
        [Key] public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public  string RoleName { get; set; }
        public Guid PermissionId { get; set; }
        public string? PermisionName { get; set; }
        public string PermisionDescription { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
