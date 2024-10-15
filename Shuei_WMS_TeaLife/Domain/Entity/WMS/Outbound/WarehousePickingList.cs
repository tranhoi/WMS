using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("WarehousePickingList")]
public partial class WarehousePickingList : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string PickNo { get; set; }

    public string ShipmentNo { get; set; }

    public string ShippingAgent { get; set; }

    public int? TenantId { get; set; }

    public string SalesNo { get; set; }

    public string Location { get; set; }

    public string PersonInCharge { get; set; }

    public DateOnly? PickedDate { get; set; }
    public EnumOrderStatus Status { get; set; } = EnumOrderStatus.Draft;
}