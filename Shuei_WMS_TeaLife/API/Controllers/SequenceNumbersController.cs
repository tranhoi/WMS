using API.Controllers.Base;
using Application.Extentions;
using Application.Services;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SequenceNumbersController : BaseController<Guid, NumberSequences>, INumberSequences
    {
        private readonly Repository _repository;

        public SequenceNumbersController(Repository repository = null) : base(repository.SNumberSequences)
        {
            _repository = repository;
        }
        [HttpGet(ApiRoutes.NumberSequences.GetNumberSequenceByType)]
        public async Task<Result<NumberSequences>> GetNumberSequenceByType(string type)
        {
            var result = await _repository.SNumberSequences.GetNumberSequenceByType(type);
            return result;
        }

        [HttpPost(ApiRoutes.NumberSequences.IncreaseNumberSequenceByType)]
        public async Task<Result<bool>> IncreaseNumberSequenceByType([Path] string type)
        {
            var result = await _repository.SNumberSequences.IncreaseNumberSequenceByType(type);
            return result;
        }
    }
}
