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
    public class WarehouseReceiptOrderController : BaseController<Guid, WarehouseReceiptOrder>,IWarehouseReceiptOrder
    {
        readonly Repository _repository;

        public WarehouseReceiptOrderController(Repository repository = null!):base(repository.SWarehouseReceiptOrders)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseReceiptOrder.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseReceiptOrder>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            return await _repository.SWarehouseReceiptOrders.GetByMasterCodeAsync(receiptNo);
        }
    }
}
