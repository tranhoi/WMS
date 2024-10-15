﻿using API.Controllers.Base;
using Application.Extentions;
using Application.Models;
using Application.Services;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Infrastructure.Data;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : BaseController<Guid, Device>, IDevices
    {
        readonly Repository _repository;

        public DevicesController(Repository repository = null) : base(repository.SDevices)
        {
            _repository = repository;
        }
    }
}
