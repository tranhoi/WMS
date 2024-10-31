

using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class WarehouseReceiptOrderDto : GenericEntity
    {
        public Guid Id { get; set; }

        public string? ReceiptNo { get; set; }


        public string? Location { get; set; }

        public DateOnly? ExpectedDate { get; set; }


        public int TenantId { get; set; }
        public int? ScheduledArrivalNumber { get; set; }

        public string? DocumentNo { get; set; }

        public int SupplierId { get; set; }


        public string? PersonInCharge { get; set; }

        public string? ConfirmedBy { get; set; }

        public DateOnly? ConfirmedDate { get; set; }

        public List<WarehouseReceiptOrderLineDto> WarehouseReceiptOrderLines { get; set; } = new();
        public string? LocationName { get; set; }
        public string? TenantFullName { get; set; }
        public string? SupplierName { get; set; }
        public string? PersonInChargeName { get; set; }
        public EnumReceiptOrderStatus? Status { get; set; } = EnumReceiptOrderStatus.Draft;
    }
}
