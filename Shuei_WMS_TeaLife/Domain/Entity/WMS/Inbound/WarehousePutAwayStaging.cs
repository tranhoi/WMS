using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Inbound;

[Table("WarehousePutAwayStaging")]
public class WarehousePutAwayStaging
{
    [Key] public Guid Id { get; set; }

    public string PutAwayNo { get; set; }

    public string ProductCode { get; set; }

    public string Unit { get; set; }

    public double? JournalQty { get; set; }

    public double? TransQty { get; set; }

    public string Bin { get; set; }

    public string Status { get; set; }

    public bool? IsDelete { get; set; }

    public string CreateOperatorId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UpdateOperatorId { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string LotNo { get; set; }
}