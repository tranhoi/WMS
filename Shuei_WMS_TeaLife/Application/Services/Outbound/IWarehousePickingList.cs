using Application.DTOs;
using Application.DTOs.Request.Picking;
using Application.DTOs.Request.shipment;
using Application.Extentions;
using Application.Services.Base;

using RestEase;
using System.Web.Mvc;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePickingList.BasePath)]
    public interface IWarehousePickingList : IRepository<Guid, WarehousePickingList>
    {
        [Get(ApiRoutes.WarehousePickingList.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePickingList>>> GetByMasterCodeAsync([Path] string pickNo);

        [Put(ApiRoutes.WarehousePickingList.GetWarehousePickingDTOAsync)]
        Task<Result<List<WarehousePickingDTO>>> GetWarehousePickingDTOAsync([Body] PickingListSearchRequestDto model);

        [Delete(ApiRoutes.WarehousePickingList.DeletePickingAsync)]
        Task<Result> DeletePickingAsync([Path] string pickNo);

        [Patch(ApiRoutes.WarehousePickingList.CompletePickingAsync)]
        Task<Result> CompletePickingAsync([Path] string pickNo);

        [Put(ApiRoutes.WarehousePickingList.SyncToHTAsync)]
        Task<Result<List<WarehousePickingLineDTO>>> SyncToHTAsync([Body] List<WarehousePickingLineDTO> model);
    }
}
