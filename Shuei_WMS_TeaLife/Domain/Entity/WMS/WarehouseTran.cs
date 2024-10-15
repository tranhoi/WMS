using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS;

[Table("WarehouseTrans")]
public class WarehouseTran : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string ProductCode { get; set; }

    public double Qty { get; set; }

    //public DateOnly? TransDate { get; set; }
    public DateOnly DatePhysical { get; set; }

    public EnumWarehouseTransType TransType { get; set; }=EnumWarehouseTransType.Receipt;

    public string TransNumber { get; set; }

    public string? Location { get; set; }

    public string? Bin { get; set; }

    public string? LotNo { get; set; }

    public string? Remarks { get; set; }

    public EnumStatusIssue? StatusIssue { get; set; } = EnumStatusIssue.None;

    public EnumStatusReceipt? StatusReceipt { get; set; }=EnumStatusReceipt.None;
    public Guid? TransId { get; set; }
    public Guid? TransLineId { get; set; }
    public string? PutAwayNo { get; set; }
    public string? PickingNo { get; set; }
    public string? PackingNo { get; set; }

    public EnumStatus Status { get; set; } = EnumStatus.Activated;
}