using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.WMS.Inbound;
using Domain.Entity.WMS.Inbound;
using Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class WarehousePutAwayStagingService : GenericService<WarehousePutAwayStaging>, IWarehousePutAwayStagingService
{
    private readonly IRepositoryAsync<WarehousePutAwayStaging, Guid> _repository;

    public WarehousePutAwayStagingService(IRepositoryAsync<WarehousePutAwayStaging, Guid> repository) : base(repository)
    {
        _repository = repository;
    }

    // ... existing methods ...

    public async Task<IResult<IEnumerable<WarehousePutAwayStaging>>> GetByPutAwayLineIdAsync(Guid putAwayLineId)
    {
        try
        {
            var stagingData = await _repository.Entities
                .Where(x => x.PutAwayLineId == putAwayLineId)
                .ToListAsync();

            return await Result<IEnumerable<WarehousePutAwayStaging>>.SuccessAsync(stagingData);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<WarehousePutAwayStaging>>.FailAsync($"An error occurred while retrieving staging data: {ex.Message}");
        }
    }
}
