using RestEase;
using Application.DTOs;
using Application.Extentions;
using Application.DTOs.Request;
using Application.Services.Base;
using Application.Extentions.Pagings;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehouseShipment.BasePath)]
    public interface IWarehouseShipment : IRepository<Guid, WarehouseShipment>
    {
        [Post(ApiRoutes.WarehouseShipment.CreateAsync)]
        Task<Result<WarehouseShipmentDto>> CreateWarehouseShipmentAsync([Body] WarehouseShipmentDto model);
        
        [Post(ApiRoutes.WarehouseShipment.UpdateAsync)]
        Task<Result<WarehouseShipmentDto>> UpdateWarehouseShipmentAsync([Body] WarehouseShipmentDto model);
        
        [Post(ApiRoutes.WarehouseShipment.SearchAsync)]
        Task<Result<PageList<WarehouseShipmentDto>>> SearchWhShipments([Body]QueryModel<WarehouseShipmentSearchModel> model);

        [Get(ApiRoutes.WarehouseShipment.GetAsync)]
        Task<Result<WarehouseShipmentDto>> GetShipmentByIdAsync([Path] Guid id);

        [Post(ApiRoutes.WarehouseShipment.DeleteAsync)]
        Task<Result<bool>> DeleteShipmentAsync([Path] Guid id);

        [Post(ApiRoutes.WarehouseShipment.ConfirmShipmentAsync)]
        Task<Result<bool>> ConfirmShipmentAsync([Path] Guid id);

        [Post(ApiRoutes.WarehouseShipment.CreatePickingAsync)]
        Task<Result<bool>> CreatePickingAsync([Body] SubmitCompletedShipmentDto data);
        
        [Post(ApiRoutes.WarehouseShipment.CheckMultipleShipmentCreatePicking)]
        Task<Result<bool>> CheckMultipleShipmentCreatePicking([Body] List<Guid> ids);
    }
}
