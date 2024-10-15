using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Inbound;

[Table("WarehousePutAwayStaging")]
public class WarehousePutAwayStaging : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string PutAwayNo { get; set; }

    public string ProductCode { get; set; }

    public string Unit { get; set; }

    public double? JournalQty { get; set; }

    public double? TransQty { get; set; }

    public string Bin { get; set; }
    public string LotNo { get; set; }
    public Guid ReceiptLineId { get; set; }
    public EnumStatusStaging Status { get; set; } = EnumStatusStaging.Pending;
}