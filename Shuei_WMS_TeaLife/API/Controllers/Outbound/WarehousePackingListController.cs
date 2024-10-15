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
    public class WarehousePackingListController : BaseController<Guid, WarehousePackingList>, IWarehousePackingList
    {
        readonly Repository _repository;

        public WarehousePackingListController(Repository repository = null!):base(repository.SWarehousePackingList) 
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePackingList.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePackingList>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            return await _repository.SWarehousePackingList.GetByMasterCodeAsync(shipmentNo);
        }
    }
}
