using Domain.Entity.WMS;
using Domain.Enums;

namespace Application.DTOs
{
    public class WarehousePutAwayDto : GenericEntity
    {
        public Guid Id { get; set; }

        public string PutAwayNo { get; set; } = string.Empty;
        public string ReceiptNo { get; set; }

        public string? Description { get; set; }

        public int TenantId { get; set; }

        public DateOnly? TransDate { get; set; }

        public DateOnly? DocumentDate { get; set; }

        public string? DocumentNo { get; set; }

        public string Location { get; set; }

        public DateOnly? PostedDate { get; set; }

        public string? PostedBy { get; set; }

        IEnumerable<WarehousePutAwayLineDto> warehousePutAwayLines { get; set; }
        public IEnumerable<WarehousePutAwayLineDto> WarehousePutAwayLines { get; set; }
        public EnumStatus Status { get; set; }
    }
}
