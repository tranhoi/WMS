using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Outbound;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePackingLine.BasePath)]
    public interface IWarehousePackingLine : IRepository<Guid, WarehousePackingLine>
    {
        [Get(ApiRoutes.WarehousePackingLine.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePackingLine>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
