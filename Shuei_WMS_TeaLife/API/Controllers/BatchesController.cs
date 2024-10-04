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
    public class BatchesController : BaseController<Guid, Batches>, IBatches
    {
        private readonly Repository _repository;

        public BatchesController(Repository repository = null) : base(repository.SBatches)
        {
            _repository = repository;
        }
    }
}
