using API.Controllers.Base;
using Application.DTOs;
using Application.Extentions;
using Application.Services.Inbound;
using Domain.Entity.WMS.Inbound;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousePutAwayController : BaseController<Guid, WarehousePutAway>, IWarehousePutAway
    {
        readonly Repository _repository;

        public WarehousePutAwayController(Repository repository = null!) : base(repository.SWarehousePutAways)
        {
            _repository = repository;
        }
        [HttpGet(ApiRoutes.WarehousePutAway.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePutAway>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            return await _repository.SWarehousePutAways.GetByMasterCodeAsync(putAwayNo);
        }

        [HttpPost(ApiRoutes.WarehousePutAway.InsertWarehousePutAwayOrder)]
        public async Task<Result<IEnumerable<WarehousePutAwayDto>>> InsertWarehousePutAwayOrder([Body] IEnumerable<WarehousePutAwayDto> request) => await _repository.SWarehousePutAways.InsertWarehousePutAwayOrder(request);
    }
}
