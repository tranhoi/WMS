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
    public class WarehouseReceiptStagingController : BaseController<Guid, WarehouseReceiptStaging>, IWarehouseReceiptStaging
    {
        readonly Repository _repository;

        public WarehouseReceiptStagingController(Repository repository = null!) : base(repository.SWarehouseReceiptStagings)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseReceiptStaging.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseReceiptStaging>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            return await _repository.SWarehouseReceiptStagings.GetByMasterCodeAsync(receiptNo);
        }
        [HttpGet(ApiRoutes.WarehouseReceiptStaging.GetByReceiptLineIdAsync)]
        public async Task<Result<WarehouseReceiptStaging>> GetByReceiptLineIdAsync([Path] Guid receiptLineId)
        {
            return await _repository.SWarehouseReceiptStagings.GetByReceiptLineIdAsync(receiptLineId);
        }
    }
}
