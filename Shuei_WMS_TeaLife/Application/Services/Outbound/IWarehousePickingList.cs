using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Outbound;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePickingList.BasePath)]
    public interface IWarehousePickingList : IRepository<Guid, WarehousePickingList>
    {
        [Get(ApiRoutes.WarehousePickingList.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePickingList>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
