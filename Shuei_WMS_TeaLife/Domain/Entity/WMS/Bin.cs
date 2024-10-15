using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS
{
    [Table("Bins")]
    public class Bin : GenericEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid LocationId { get; set; }
        public string? LocationCD { get; set; }
        public string? LocationName { get; set; }

        public string BinCode { get; set; }

        public string? Remarks { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
