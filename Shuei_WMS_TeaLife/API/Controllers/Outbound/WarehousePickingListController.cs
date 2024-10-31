using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request.Picking;
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
    public class WarehousePickingListController : BaseController<Guid, WarehousePickingList>, IWarehousePickingList
    {
        readonly Repository _repository;

        public WarehousePickingListController(Repository repository = null!) : base(repository.SWarehousePickingList)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePickingList.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePickingList>>> GetByMasterCodeAsync([Path] string pickNo)
        {
            return await _repository.SWarehousePickingList.GetByMasterCodeAsync(pickNo);
        }

        [HttpPut(ApiRoutes.WarehousePickingList.GetWarehousePickingDTOAsync)]
        public async Task<Result<List<WarehousePickingDTO>>> GetWarehousePickingDTOAsync([Body] PickingListSearchRequestDto model)
        {
            return await _repository.SWarehousePickingList.GetWarehousePickingDTOAsync(model);
        }

        [HttpDelete(ApiRoutes.WarehousePickingList.DeletePickingAsync)]
        public async Task<Result> DeletePickingAsync(string pickNo)
        {
            return await _repository.SWarehousePickingList.DeletePickingAsync(pickNo);
        }
        [HttpPatch(ApiRoutes.WarehousePickingList.CompletePickingAsync)]
        public async Task<Result> CompletePickingAsync(string pickNo)
        {
            return await _repository.SWarehousePickingList.CompletePickingAsync(pickNo);
        }

        [HttpPut(ApiRoutes.WarehousePickingList.SyncToHTAsync)]
        public async Task<Result<List<WarehousePickingLineDTO>>> SyncToHTAsync([Body] List<WarehousePickingLineDTO> model)
        {
            return await _repository.SWarehousePickingList.SyncToHTAsync(model);
        }
    }
}
