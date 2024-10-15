using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Inbound;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehousePutAway.BasePath)]
    public interface IWarehousePutAway : IRepository<Guid, WarehousePutAway>
    {
        [Get(ApiRoutes.WarehousePutAway.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePutAway>>> GetByMasterCodeAsync([Path] string putAwayNo);

        [Post(ApiRoutes.WarehousePutAway.InsertWarehousePutAwayOrder)]
        Task<Result<IEnumerable<WarehousePutAwayDto>>> InsertWarehousePutAwayOrder([Body] IEnumerable<WarehousePutAwayDto> request);
    }
}
