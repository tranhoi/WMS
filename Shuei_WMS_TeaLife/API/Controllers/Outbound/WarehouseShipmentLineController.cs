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
    public class WarehouseShipmentLineController : BaseController<Guid, WarehouseShipmentLine>, IWarehouseShipmentLine
    {
        readonly Repository _repository;

        public WarehouseShipmentLineController(Repository repository = null!):base(repository.SWarehouseShipmentLine) 
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseShipmentLine.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseShipmentLine>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            return await _repository.SWarehouseShipmentLine.GetByMasterCodeAsync(shipmentNo);
        }
    }
}
