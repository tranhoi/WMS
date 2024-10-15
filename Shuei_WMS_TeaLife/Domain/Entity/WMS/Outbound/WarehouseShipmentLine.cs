using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("WarehouseShipmentLines")]
public partial class WarehouseShipmentLine : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string ShipmentNo { get; set; }

    public string ProductCode { get; set; }

    public string Unit { get; set; }

    public double? ShipmentQty { get; set; }

    public string Location { get; set; }

    public string Bin { get; set; }
    public EnumOrderStatus Status { get; set; } = EnumOrderStatus.Draft;
}