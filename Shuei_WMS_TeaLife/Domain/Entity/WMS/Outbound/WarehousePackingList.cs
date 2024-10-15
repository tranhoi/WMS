using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("WarehousePackingList")]
public partial class WarehousePackingList : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string PackingNo { get; set; }

    public DateOnly? PackingDate { get; set; }

    public string ShipmentNo { get; set; }

    public string TenantId { get; set; }

    public string Location { get; set; }

    public string ShippingCarrier { get; set; }

    public string ShippingAdress { get; set; }
    public EnumOrderStatus Status { get; set; } = EnumOrderStatus.Draft;
}