using Application.Extentions;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategories account) : ControllerBase
    {
        [HttpGet(ApiRoutes.Categories.GetUsers)]
        public async Task<ActionResult<Result<List<System.Web.Mvc.SelectListItem>>>> GetUserDropdown()
        {
            var result = await account.GetUserDropdown();
            return Ok(result);
        }

        [HttpGet(ApiRoutes.Categories.GetBinByLocationId)]
        public async Task<ActionResult<Result<List<System.Web.Mvc.SelectListItem>>>> GetBinByLocation(string locationId)
        {
            var result = await account.GetBinByLocation(locationId);
            return Ok(result);
        }
    }
}
