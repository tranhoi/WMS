using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehouseReceiptOrderLine.BasePath)]
    public interface IWarehouseReceiptOrderLine:IRepository<Guid, WarehouseReceiptOrderLine>
    {
        [Get(ApiRoutes.WarehouseReceiptOrderLine.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseReceiptOrderLine>>> GetByMasterCodeAsync([Path] string receiptNo);
    }
}
