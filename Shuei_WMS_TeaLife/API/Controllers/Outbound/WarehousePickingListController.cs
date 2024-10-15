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
    public class WarehousePickingListController : BaseController<Guid, WarehousePickingList>, IWarehousePickingList
    {
        readonly Repository _repository;

        public WarehousePickingListController(Repository repository = null!):base(repository.SWarehousePickingList) 
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePickingList.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePickingList>>> GetByMasterCodeAsync([Path] string pickNo)
        {
            return await _repository.SWarehousePickingList.GetByMasterCodeAsync(pickNo);
        }
    }
}
