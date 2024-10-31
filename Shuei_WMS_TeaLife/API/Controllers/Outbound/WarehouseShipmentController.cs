using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Extentions.Pagings;
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
    public class WarehouseShipmentController : BaseController<Guid, WarehouseShipment>, IWarehouseShipment
    {
        readonly Repository _repository;

        public WarehouseShipmentController(Repository repository = null!):base(repository.SWarehouseShipment) 
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CreateAsync)]
        public async Task<Result<WarehouseShipmentDto>> CreateWarehouseShipmentAsync([Body] WarehouseShipmentDto model)
        {
            return await _repository.SWarehouseShipment.CreateWarehouseShipmentAsync(model);
        }

        //[HttpGet(ApiRoutes.WarehouseShipment.GetByMasterCodeAsync)]
        //public async Task<Result<List<WarehouseShipment>>> GetByMasterCodeAsync([Path] string shipmentNo)
        //{
        //    return await _repository.SWarehouseShipment.GetByMasterCodeAsync(shipmentNo);
        //}

        [HttpPost(ApiRoutes.WarehouseShipment.SearchAsync)]
        public async Task<Result<PageList<WarehouseShipmentDto>>> SearchWhShipments(QueryModel<WarehouseShipmentSearchModel> model)
        {
            return await _repository.SWarehouseShipment.SearchWhShipments(model);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.UpdateAsync)]
        public async Task<Result<WarehouseShipmentDto>> UpdateWarehouseShipmentAsync(WarehouseShipmentDto model)
        {
            return await _repository.SWarehouseShipment.UpdateWarehouseShipmentAsync(model);
        }

        [HttpGet(ApiRoutes.WarehouseShipment.GetAsync)]
        public async Task<Result<WarehouseShipmentDto>> GetShipmentByIdAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.GetShipmentByIdAsync(id);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.DeleteAsync)]
        public async Task<Result<bool>> DeleteShipmentAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.DeleteShipmentAsync(id);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.ConfirmShipmentAsync)]
        public async Task<Result<bool>> ConfirmShipmentAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.ConfirmShipmentAsync(id);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CreatePickingAsync)]
        public async Task<Result<bool>> CreatePickingAsync([Body] SubmitCompletedShipmentDto data)
        {
            return await _repository.SWarehouseShipment.CreatePickingAsync(data);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CheckMultipleShipmentCreatePicking)]
        public async Task<Result<bool>> CheckMultipleShipmentCreatePicking([Body] List<Guid> ids)
        {
            return await _repository.SWarehouseShipment.CheckMultipleShipmentCreatePicking(ids);
        }
    }
}
