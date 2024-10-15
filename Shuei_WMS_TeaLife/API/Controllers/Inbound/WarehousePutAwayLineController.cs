using API.Controllers.Base;
using Application.DTOs;
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
    public class WarehousePutAwayLineController : BaseController<Guid, WarehousePutAwayLine>, IWarehousePutAwayLine
    {
        readonly Repository _repository;

        public WarehousePutAwayLineController(Repository repository = null!) : base(repository.SWarehousePutAwayLines)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePutAwayLine.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePutAwayLine>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            return await _repository.SWarehousePutAwayLines.GetByMasterCodeAsync(putAwayNo);
        }

        [HttpGet(ApiRoutes.WarehousePutAwayLine.GetLabelById)]
        public async Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] Guid id)
        {
            return await _repository.SWarehousePutAwayLines.GetLabelByIdAsync(id);
        }
        [HttpGet(ApiRoutes.WarehousePutAwayLine.GetLabelByPutAwayNo)]
        public async Task<List<LabelInfoDto>> GetLabelByPutAwayNo([Path] string putAwayNo)
        {
            return await _repository.SWarehousePutAwayLines.GetLabelByPutAwayNo(putAwayNo);
        }
    }
}
