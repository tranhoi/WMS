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
    [Table("VendorBillingDetail")]
    public class VendorBillingDetail : GenericEntity
    {
        [Key] public int Id { get; set; }
        public int VendorBillingHeaderId { get; set; }
        public string CompanyId { get; set; }
        public string? VendorCode { get; set; }
        public string? BillingPeriod { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderId { get; set; }
        public string? SalesProductCode { get; set; }
        public string? CustomerProductCode { get; set; }
        public string? CHannelMasterCode { get; set; }
        public string? Currency { get; set; }
        public double? CurrencyRate { get; set; } = 0;
        public double? PurchaseUnitPrice { get; set; } = 0;
        public double? MallFee { get; set; } = 0;
        public double? DiscountAmount { get; set; } = 0;
        public double? JamFeeExcludingTax { get; set; } = 0;
        public double? JamFeeTaxAmount { get; set; } = 0;
        public double? MallShippingFee { get; set; } = 0;
        public double? ShippingFee { get; set; } = 0;
        public double? SubTotal { get; set; } = 0;
        public string? DataKey { get; set; }
        public double? SlsLgsShippingFee { get; set; } = 0;
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
