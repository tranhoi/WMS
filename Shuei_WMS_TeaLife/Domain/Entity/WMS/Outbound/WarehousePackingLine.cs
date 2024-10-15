using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("WarehousePackingLines")]
public partial class WarehousePackingLine : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string ShipmentNo { get; set; }

    public string ProductNo { get; set; }

    public string Unit { get; set; }

    public double? ShipmentOrderQty { get; set; }

    public double? PickedQty { get; set; }

    public double? PackedQty { get; set; }

    public string Location { get; set; }

    public string Bin { get; set; }
    public EnumOrderStatus Status { get; set; } = EnumOrderStatus.Draft;
}