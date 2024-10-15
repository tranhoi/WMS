using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS
{
    public class GenericEntity
    {
        public string? CreateOperatorId { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? UpdateOperatorId { get; set; }

        public DateTime? UpdateAt { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
}
