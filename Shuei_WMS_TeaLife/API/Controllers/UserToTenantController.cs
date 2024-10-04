using API.Controllers.Base;
using Application.Extentions;
using Application.Services;
using Domain.Entity.WMS;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserToTenantController : BaseController<Guid, UserToTenant>, IUserToTenant
    {
        readonly Repository _repository;

        public UserToTenantController(Repository repository = null!):base(repository.SUserToTenant)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.UserToTenant.GetByUserId)]
        public async Task<Result<List<UserToTenant>>> GetByUserIdAsync(string userId)
        {
            return await _repository.SUserToTenant.GetByUserIdAsync(userId);    
        }
    }
}
