using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehouseShipmentLine.BasePath)]
    public interface IWarehouseShipmentLine : IRepository<Guid, WarehouseShipmentLine>
    {
        [Get(ApiRoutes.WarehouseShipmentLine.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseShipmentLine>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
