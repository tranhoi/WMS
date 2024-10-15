using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Outbound;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.ShippingBox.BasePath)]
    public interface IShippingBox : IRepository<Guid, ShippingBox>
    {
        //[Get(ApiRoutes.WarehouseShipmentLine.GetByMasterCodeAsync)]
        //Task<Result<List<ShippingBox>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
