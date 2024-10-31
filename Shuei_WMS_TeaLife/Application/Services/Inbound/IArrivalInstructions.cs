using Application.Extentions;
using Application.Services.Base;

using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.ArrivalInstructions.BasePath)]
    public interface IArrivalInstructions : IRepository<int, ArrivalInstruction>
    {
    }
}
