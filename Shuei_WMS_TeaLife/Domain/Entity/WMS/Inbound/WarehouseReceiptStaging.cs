using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Inbound;

[Table("WarehouseReceiptStaging")]
public class WarehouseReceiptStaging
{
    [Key] public Guid Id { get; set; }

    public string ReceiptNo { get; set; }

    public string ProductCode { get; set; }

    public string Unit { get; set; }

    public double? OrderQty { get; set; }

    public double? TransQty { get; set; }

    public string Bin { get; set; }

    public string CreateOperatorId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UpdateOperatorId { get; set; }

    public DateTime? UpdateAt { get; set; }

    public bool? IsDeleted { get; set; }

    public string Status { get; set; }

    public string LotNo { get; set; }

    public DateOnly? ExpirationDate { get; set; }
}