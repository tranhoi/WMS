

namespace Application.DTOs.Request
{
    public class WarehouseShipmentSearchModel
    {
        public string? ShipmentNo { get; set; }
        public string? SalesNo { get; set; }
        public int? TenantId { get; set; }
        public Guid LocationId { get; set; }
        public DateOnly? DeliveryDateFrom { get; set; }
        public DateOnly? DeliveryDateTo { get; set; }
        public string? BinId { get; set; }
        public EnumShipmentOrderStatus? Status { get; set; }
    }
}
