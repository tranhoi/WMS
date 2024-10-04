using Application.Extentions;
using Application.Services.Base;
using Domain.Entity.Commons;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Suppliers
{
    [BasePath(ApiRoutes.Suppliers.BasePath)]
    public interface ISuppliers:IRepository<int,Supplier>
    {
    }
}
