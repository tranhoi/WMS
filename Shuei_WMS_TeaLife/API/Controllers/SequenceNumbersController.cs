using API.Controllers.Base;
using Application.Services;
using Application.Services.Base;
using Domain.Entity.WMS;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
