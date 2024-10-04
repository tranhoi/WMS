using API.Controllers.Base;
using Application.Services.Suppliers;
using Domain.Entity.Commons;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Vendors
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : BaseController<int,Supplier>,ISuppliers
    {
        readonly Repository _repository;

        public SuppliersController(Repository repository =null!):base(repository.SSuppliers) 
        {
            _repository = repository;
        }
    }
}
