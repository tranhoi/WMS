using Domain.Entity.WMS;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class WarehouseReceiptOrderDto : GenericEntity
    {
        public Guid Id { get; set; }

        public string ReceiptNo { get; set; }

        [Required]
        public string Location { get; set; }

        public DateOnly? ExpectedDate { get; set; }

        [Required]
        public int TenantId { get; set; }
        public int? ScheduledArrivalNumber { get; set; }

        public string? DocumentNo { get; set; }

        [Required]
        public int SupplierId { get; set; }

        public string? PersonInCharge { get; set; }

        public string? ConfirmedBy { get; set; }

        public DateOnly? ConfirmedDate { get; set; }

        IEnumerable<WarehouseReceiptOrderLineDto> warehouseReceiptOrderLines { get; set; }

        public IEnumerable<WarehouseReceiptOrderLineDto> WarehouseReceiptOrderLines
        {
            get { return warehouseReceiptOrderLines; }
            set { warehouseReceiptOrderLines = value; }
        }
        public string? LocationName { get; set; }
        public string? TenantFullName { get; set; }
        public EnumReceiptStatus? Status { get; set; } = EnumReceiptStatus.Draft;
    }
}
