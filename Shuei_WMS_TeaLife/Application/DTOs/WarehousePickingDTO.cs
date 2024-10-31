

namespace Application.DTOs
{
    public class WarehousePickingDTO
    {
        public string PickNo { get; set; }
        public string? Location { get; set; }
        public DateOnly? PickedDate { get; set; }
        public DateOnly? PlanShipDate { get; set; }
        public string? ShipmentNo { get; set; }
        public string? PersonInCharge { get; set; }
        public EnumShipmentOrderStatus? Status { get; set; } = EnumShipmentOrderStatus.Draft;
    }
}