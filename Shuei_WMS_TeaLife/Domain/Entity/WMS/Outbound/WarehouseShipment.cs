using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("WarehouseShipments")]
public partial class WarehouseShipment : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string ShipmentNo { get; set; }

    public string SalesNo { get; set; }

    public int TenantId { get; set; }

    public string Location { get; set; }

    public string PlanShipDate { get; set; }

    public string PersonInCharge { get; set; }

    public string ShippingCarrierCode { get; set; }

    public string ShippingAddress { get; set; }

    public string Telephone { get; set; }

    public string TrackingNo { get; set; }

    public string Email { get; set; }
    public EnumOrderStatus Status { get; set; } = EnumOrderStatus.Draft;
}