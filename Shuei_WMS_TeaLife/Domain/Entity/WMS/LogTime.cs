using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS
{
    [Table("LogTime")]
    public class LogTime
    {
        [Key] public Guid Id { get; set; }
        public string? LogName { get; set; }
        public double? EslapseTime { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
