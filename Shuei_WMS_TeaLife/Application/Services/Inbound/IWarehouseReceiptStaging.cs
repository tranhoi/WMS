using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Inbound;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehouseReceiptStaging.BasePath)]
    public interface IWarehouseReceiptStaging:IRepository<Guid, WarehouseReceiptStaging>
    {
        [Get(ApiRoutes.WarehouseReceiptStaging.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseReceiptStaging>>> GetByMasterCodeAsync([Path] string receiptNo);

        [Get(ApiRoutes.WarehouseReceiptStaging.GetByReceiptLineIdAsync)]
        Task<Result<WarehouseReceiptStaging>> GetByReceiptLineIdAsync([Path] Guid receiptLineId);
    }
}
