﻿using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [BasePath(ApiRoutes.Devices.BasePath)]
    public interface IDevices : IRepository<Guid, Device>
    {

    }
}
