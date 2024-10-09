using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Outbound;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePackingList.BasePath)]
    public interface IWarehousePackingList : IRepository<Guid, WarehousePackingList>
    {
        [Get(ApiRoutes.WarehousePackingList.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePackingList>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
