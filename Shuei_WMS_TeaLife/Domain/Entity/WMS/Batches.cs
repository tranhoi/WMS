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
    [Table("Batches")]
    public class Batches : GenericEntity
    {
        [Key] public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public int TenantId { get; set; }
        public string LotNo { get; set; }
        public DateOnly? ManufacturingDate { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
