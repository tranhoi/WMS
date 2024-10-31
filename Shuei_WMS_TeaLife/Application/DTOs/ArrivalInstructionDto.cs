using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ArrivalInstructionDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ScheduledArrivalNumber { get; set; }
        public DateTime ScheduledArrivalDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string DataKey { get; set; }
    }
}
