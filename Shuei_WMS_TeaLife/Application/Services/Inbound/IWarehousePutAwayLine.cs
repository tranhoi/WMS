using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Inbound;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehousePutAwayLine.BasePath)]
    public interface IWarehousePutAwayLine : IRepository<Guid, WarehousePutAwayLine>
    {
        [Get(ApiRoutes.WarehousePutAwayLine.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePutAwayLine>>> GetByMasterCodeAsync([Path] string putAwayNo);
    }
}
