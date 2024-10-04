using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Inbound;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehousePutAwayStaging.BasePath)]
    public interface IWarehousePutAwayStaging:IRepository<Guid, WarehousePutAwayStaging>
    {
        [Get(ApiRoutes.WarehousePutAwayStaging.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePutAwayStaging>>> GetByMasterCodeAsync([Path] string putAwayNo);
    }
}
