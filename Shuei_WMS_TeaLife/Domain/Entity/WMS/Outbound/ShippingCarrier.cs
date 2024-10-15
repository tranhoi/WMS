using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("ShippingCarriers")]
public partial class ShippingCarrier : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string? ShippingCarrierCode { get; set; }
    public string ShippingCarrierName { get; set; }
    public EnumStatus Status { get; set; } = EnumStatus.Activated;
}