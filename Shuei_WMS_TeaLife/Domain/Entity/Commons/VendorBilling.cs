using Domain.Entity.WMS;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Commons
{
    [Table("VendorBilling")]
    public class VendorBilling : GenericEntity
    {
        [Key] public int Id { get; set; }
        public string? CompanyId { get; set; }
        public string? VendorCode { get; set; }
        public string? BillingPeriod { get; set; }
        public double? Total { get; set; } = 0;
        public string? DataKey { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
