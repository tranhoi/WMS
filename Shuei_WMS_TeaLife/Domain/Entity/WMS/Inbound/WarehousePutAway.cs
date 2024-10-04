using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Inbound;

[Table("WarehousePutAways")]
public class WarehousePutAway
{
    [Key] public Guid Id { get; set; }

    public string PutAwayNo { get; set; }
    public string ReceiptNo { get; set; }

    public string Description { get; set; }

    public int TenantId { get; set; }

    public DateOnly TransDate { get; set; }

    public DateOnly? DocumentDate { get; set; }

    public string DocumentNo { get; set; }

    public string Location { get; set; }

    public DateOnly? PostedDate { get; set; }

    public string PostedBy { get; set; }

    public string Status { get; set; }

    public bool? IsDelete { get; set; }

    public string CreateOperatorId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UpdateOperatorId { get; set; }

    public DateTime? UpdateAt { get; set; }
}