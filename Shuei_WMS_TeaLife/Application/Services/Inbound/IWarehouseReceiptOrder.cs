using Application.DTOs;
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

        [Post(ApiRoutes.WarehouseReceiptOrder.InsertWarehouseReceiptOrder)]
        Task<Result<WarehouseReceiptOrder>> InsertWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request);

        [Post(ApiRoutes.WarehouseReceiptOrder.UpdateWarehouseReceiptOrder)]
        Task<Result<WarehouseReceiptOrder>> UpdateWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request);

        [Get(ApiRoutes.WarehouseReceiptOrder.GetReceiptOrderAsync)]
        Task<Result<WarehouseReceiptOrderDto>> GetReceiptOrderAsync(string receiptNo);

        [Get(ApiRoutes.WarehouseReceiptOrder.GetReceiptOrderListAsync)]
        Task<Result<List<WarehouseReceiptOrderDto>>> GetReceiptOrderListAsync();
    }
}
