using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS
{
    [Table("UserToTenant")]
    public class UserToTenant : GenericEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public int TenantId { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;

    }
}
