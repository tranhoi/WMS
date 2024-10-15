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
    public class WarehouseReceiptOrderLineController : BaseController<Guid, WarehouseReceiptOrderLine>, IWarehouseReceiptOrderLine
    {
        readonly Repository _repository;

        public WarehouseReceiptOrderLineController(Repository repository = null!):base(repository.SWarehouseReceiptOrderLines)
        {
            _repository = repository;
        }
        [HttpGet(ApiRoutes.WarehouseReceiptOrderLine.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseReceiptOrderLine>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            return await _repository.SWarehouseReceiptOrderLines.GetByMasterCodeAsync(receiptNo);
        }
    }
}
