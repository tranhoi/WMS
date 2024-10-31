using API.Controllers.Base;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseTranController : BaseController<Guid, WarehouseTran>,IWarehouseTran
    {
        readonly Repository _repository;

        public WarehouseTranController(Repository repository = null!):base(repository.SWarehouseTrans)
        {
            _repository = repository;
        }
    }
}
