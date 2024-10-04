using API.Controllers.Base;
using Application.Services.Inbound;
using Domain.Entity.Commons;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArrivalInstructionsController : BaseController<int, ArrivalInstruction>,IArrivalInstructions
    {
        readonly Repository _repository;

        public ArrivalInstructionsController(Repository repository = null!):base(repository.SArrivalInstructions)
        {
            _repository = repository;
        }
    }
}
