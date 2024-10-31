using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.WarehouseTran.BasePath)]
    public interface IWarehouseTran : IRepository<Guid, WarehouseTran>
    {
    }
}
