using Application.DTOs;
using Application.DTOs.Request.shipment;
using Application.Extentions;
using Application.Services.Outbound;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Outbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PackingListController : ControllerBase, IPackingList
    {
        readonly Repository _repository;

        public PackingListController(Repository repository)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.PackingList.CompletePackingAsync)]
        public async Task<Result<string>> CompletePackingAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            return await _repository.SPackingList.CompletePackingAsync(model);
        }

        [HttpPost(ApiRoutes.PackingList.GetDataMasterAsync)]
        public async Task<Result<List<WarehousePackingListDto>>> GetDataMasterAsync([Body] PackingListSearchRequestDto model)
        {
            return await _repository.SPackingList.GetDataMasterAsync(model);
        }

        [HttpPost(ApiRoutes.PackingList.UpdatePackedQtyAsync)]
        public async Task<Result<string>> UpdatePackedQtyAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            return await _repository.SPackingList.UpdatePackedQtyAsync(model);
        }
    }
}
