using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.Commons;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [BasePath(ApiRoutes.CurrencyPairSetting.BasePath)]
    public interface ICurrencyPairSetting : IRepository<int, CurrencyPairSetting>
    {
    }
}
