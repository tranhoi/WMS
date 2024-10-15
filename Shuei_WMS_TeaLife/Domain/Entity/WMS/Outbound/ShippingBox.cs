using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS.Outbound;

[Table("ShippingBoxes")]
public partial class ShippingBox : GenericEntity
{
    [Key] public Guid Id { get; set; }

    public string? BoxName { get; set; }

    public string? BoxType { get; set; }

    public double? Length { get; set; }

    public double? Width { get; set; }

    public double? Height { get; set; }

    public double? MaxWeight { get; set; }
    public EnumStatus Status { get; set; } = EnumStatus.Activated;
}