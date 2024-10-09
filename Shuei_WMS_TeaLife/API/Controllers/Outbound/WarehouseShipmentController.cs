using API.Controllers.Base;
using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
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

        [HttpGet(ApiRoutes.WarehouseShipment.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseShipment>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            return await _repository.SWarehouseShipment.GetByMasterCodeAsync(shipmentNo);
        }
    }
}
