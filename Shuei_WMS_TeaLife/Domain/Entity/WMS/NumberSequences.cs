using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS
{
    [Table("NumberSequences")]
    public class NumberSequences : GenericEntity
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Receipt, Putaway, Shipment, Pick, Pack, Movement, Transfer, Counting.
        /// </summary>
        public string? JournalType { get; set; }
        public string? Prefix { get; set; }
        public int? SequenceLength { get; set; }
        public int? CurrentSequenceNo { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
