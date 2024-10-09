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
    public class WarehousePackingLineController : BaseController<Guid, WarehousePackingLine>, IWarehousePackingLine
    {
        readonly Repository _repository;

        public WarehousePackingLineController(Repository repository = null!):base(repository.SWarehousePackingLine) 
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePackingLine.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePackingLine>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            return await _repository.SWarehousePackingLine.GetByMasterCodeAsync(shipmentNo);
        }
    }
}
