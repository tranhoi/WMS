using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.WMS.Inbound;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehousePutAwayLine.BasePath)]
    public interface IWarehousePutAwayLine : IRepository<Guid, WarehousePutAwayLine>
    {
        [Get(ApiRoutes.WarehousePutAwayLine.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePutAwayLine>>> GetByMasterCodeAsync([Path] string putAwayNo);

        [Get(ApiRoutes.WarehousePutAwayLine.GetLabelById)]
        Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] Guid id);
        [Get(ApiRoutes.WarehousePutAwayLine.GetLabelByPutAwayNo)]
        Task<List<LabelInfoDto>> GetLabelByPutAwayNo([Path] string putAwayNo);
    }
}
