using API.Controllers.Base;
using Application.Extentions;
using Application.Services.Inbound;
using Domain.Entity.Commons;
using Domain.Entity.WMS.Inbound;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousePutAwayStagingController : BaseController<Guid, WarehousePutAwayStaging>,IWarehousePutAwayStaging
    {
        readonly Repository _repository;

        public WarehousePutAwayStagingController(Repository repository = null!):base(repository.SWarehousePutAwayStagings)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePutAwayStaging.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePutAwayStaging>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            return await _repository.SWarehousePutAwayStagings.GetByMasterCodeAsync(putAwayNo);
        }
    }
}
