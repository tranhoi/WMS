using Application.Extentions;
using RestEase;
using System.Web.Mvc;

namespace Application.Services
{
    [BasePath(ApiRoutes.Categories.BasePath)]
    public interface ICategories
    {
        [Get(ApiRoutes.Categories.GetUsers)]
        Task<Result<List<SelectListItem>>> GetUserDropdown();
        [Get(ApiRoutes.Categories.GetBinByLocationId)]
        Task<Result<List<SelectListItem>>> GetBinByLocation(string? locationId = default);
    }
}
