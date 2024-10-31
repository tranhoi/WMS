using Application.DTOs;
using Application.DTOs.Request.shipment;
using Application.Extentions;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.PackingList.BasePath)]
    public interface IPackingList
    {
        [Post(ApiRoutes.PackingList.GetDataMasterAsync)]
        Task<Result<List<WarehousePackingListDto>>> GetDataMasterAsync([Body] PackingListSearchRequestDto model);

        [Post(ApiRoutes.PackingList.UpdatePackedQtyAsync)]
        Task<Result<string>> UpdatePackedQtyAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model);

        [Post(ApiRoutes.PackingList.CompletePackingAsync)]
        Task<Result<string>> CompletePackingAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model);
    }
}
