using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Enums
{
    public enum EnumStatusIssue
    {
        None = 0,
        OnOrder = 1, // Warehouse Shipment
        Picked = 2,
        Deliveried = 3
    }
}
