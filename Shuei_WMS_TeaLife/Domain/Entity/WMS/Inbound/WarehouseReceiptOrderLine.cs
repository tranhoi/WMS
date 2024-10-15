using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Inbound;

[Table("WarehouseReceiptOrderLine")]
public class WarehouseReceiptOrderLine : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string ReceiptNo { get; set; }

    [Required]
    public string ProductCode { get; set; }

    public string? UnitName { get; set; }

    public double? OrderQty { get; set; }

    public double? TransQty { get; set; }

    public string Bin { get; set; }

    public string LotNo { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public bool? Putaway { get; set; }

    public int? UnitId { get; set; }
    public EnumStatus Status { get; set; } = EnumStatus.Activated;
}