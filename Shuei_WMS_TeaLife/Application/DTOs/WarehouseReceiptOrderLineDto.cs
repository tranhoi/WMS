using Domain.Entity.WMS;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class WarehouseReceiptOrderLineDto : GenericEntity
    {
        [Key] public Guid Id { get; set; }

        public string ReceiptNo { get; set; }

        public string ProductCode { get; set; }

        public string? UnitName { get; set; }

        public double? OrderQty { get; set; }

        public double? TransQty { get; set; }

        [Required]
        public string Bin { get; set; }

        [Required]
        public string LotNo { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public bool? Putaway { get; set; }

        public int UnitId { get; set; }
        public string? ProductName { get; set; }
        public int? StockAvailableQuantity { get; set; }
        public int? NumberOfPossibleUses { get; set; }
        public int? ShelvedNumber { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
