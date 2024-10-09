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
    public class WarehousePickingLineController : BaseController<Guid, WarehousePickingLine>, IWarehousePickingLine
    {
        readonly Repository _repository;

        public WarehousePickingLineController(Repository repository = null!) : base(repository.SWarehousePickingLine)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePickingLinne.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePickingLine>>> GetByMasterCodeAsync([Path] string pickNo)
        {
            return await _repository.SWarehousePickingLine.GetByMasterCodeAsync(pickNo);
        }
    }
}
