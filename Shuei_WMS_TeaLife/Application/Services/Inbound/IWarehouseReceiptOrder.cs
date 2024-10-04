using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Inbound;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehouseReceiptOrder.BasePath)]
    public interface IWarehouseReceiptOrder:IRepository<Guid, WarehouseReceiptOrder>
    {
        [Get(ApiRoutes.WarehouseReceiptOrder.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseReceiptOrder>>> GetByMasterCodeAsync([Path] string receiptNo);
    }
}
