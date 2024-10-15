
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("ReturnOrders")]
public partial class ReturnOrder : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string ReturnOrderNo { get; set; }

    public string ShipmentNo { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string Reason { get; set; }

    public string PersonInCharge { get; set; }

    public string ShipTo { get; set; }

    public DateOnly? ShipDate { get; set; }
    public EnumStatus Status { get; set; } = EnumStatus.Activated;
}