using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.WMS.Inbound;
using Domain.Entity.WMS.Inbound;
using Shared.Wrapper;

public interface IWarehousePutAwayStagingService : IGenericService<WarehousePutAwayStaging>
{
    // ... existing methods ...

    Task<IResult<IEnumerable<WarehousePutAwayStaging>>> GetByPutAwayLineIdAsync(Guid putAwayLineId);
}
