using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS;

[Table("WarehouseTrans")]
public class WarehouseTran
{
    [Key] public Guid Id { get; set; }

    public string ProductCode { get; set; }

    public double Qty { get; set; }

    public DateOnly TransDate { get; set; }

    public string TransType { get; set; }

    public string TransNumber { get; set; }

    public string Location { get; set; }

    public string Bin { get; set; }

    public string LotNo { get; set; }

    public string Remarks { get; set; }

    public string CreateOperatorId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UpdateOperatorId { get; set; }

    public DateTime? UpdateAt { get; set; }

    public bool? IsDeleted { get; set; }

    public string Status { get; set; }

    public string StatusIssue { get; set; }

    public string StatusReceipt { get; set; }
}