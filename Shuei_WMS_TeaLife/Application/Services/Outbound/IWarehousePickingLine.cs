using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Outbound;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePickingList.BasePath)]
    public interface IWarehousePickingLine : IRepository<Guid, WarehousePickingLine>
    {
        [Get(ApiRoutes.WarehousePickingList.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePickingLine>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
