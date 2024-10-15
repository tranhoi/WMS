using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("WarehousePickingStaging")]
public partial class WarehousePickingStaging : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string PickNo { get; set; }

    public string ProductCode { get; set; }

    public string Unit { get; set; }

    public string Location { get; set; }

    public string Bin { get; set; }

    public string LotNo { get; set; }

    public double? PickQty { get; set; }

    public double? ActualQty { get; set; }

    public string ShipmentNo { get; set; }

    public string SalesNo { get; set; }

    public string ShippingCarrierCode { get; set; }

    public int? TenantId { get; set; }

    public DateOnly? OrderDate { get; set; }
    public EnumOrderStatus Status { get; set; } = EnumOrderStatus.Draft;
}