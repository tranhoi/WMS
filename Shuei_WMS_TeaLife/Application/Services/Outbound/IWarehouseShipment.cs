using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Outbound;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehouseShipment.BasePath)]
    public interface IWarehouseShipment : IRepository<Guid, WarehouseShipment>
    {
        [Get(ApiRoutes.WarehouseShipment.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseShipment>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
