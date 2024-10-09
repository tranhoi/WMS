using API.Controllers.Base;
using Application.Extentions;
using Application.Services.Outbound;
using Domain.Entity.WMS.Outbound;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Outbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingBoxController : BaseController<Guid, ShippingBox>, IShippingBox
    {
        readonly Repository _repository;

        public ShippingBoxController(Repository repository = null!):base(repository.SShippingBox) 
        {
            _repository = repository;
        }
    }
}
