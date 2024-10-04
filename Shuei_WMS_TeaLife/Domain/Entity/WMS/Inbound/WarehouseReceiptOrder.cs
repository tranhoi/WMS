using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Inbound;

[Table("WarehouseReceiptOrders")]
public class WarehouseReceiptOrder
{
    [Key] public Guid Id { get; set; }

    public string ReceiptNo { get; set; }

    public string VendorCode { get; set; }

    public string Location { get; set; }

    public DateOnly? ExpectedDate { get; set; }

    public string ArrivalType { get; set; }

    public int? TenantId { get; set; }

    public string CreateOperatorId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UpdateOperatorId { get; set; }

    public DateTime? UpdateAt { get; set; }

    public bool? IsDeleted { get; set; }

    public string Status { get; set; }
}