using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.WMS
{
    [Table("Units")]
    public class Unit : GenericEntity
    {
        [Key]
        public int Id { get; set; }
        public string UnitName { get; set; }
        public string? Description { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
