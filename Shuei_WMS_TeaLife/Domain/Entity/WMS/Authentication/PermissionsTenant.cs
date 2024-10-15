using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS.Authentication
{
    public class PermissionsTenant: GenericEntity
    {
        [Key] public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Desciption { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
