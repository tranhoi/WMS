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
    [Table("Locations")]
    public class Location : GenericEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string? LocationCD { get; set; }

        public string? LocationName { get; set; }

        public string? Abbreviation { get; set; }

        public string? Type { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Fax { get; set; }

        public string? Address { get; set; }

        public string? Remarks { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
