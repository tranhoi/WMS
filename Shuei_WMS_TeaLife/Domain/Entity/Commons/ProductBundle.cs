using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Commons;

public partial class ProductBundle
{
    [Key] public int Id { get; set; }

    public int ParentProductId { get; set; }

    public string CompanyId { get; set; }

    public string WarehouseCode { get; set; }

    public string BundleCode { get; set; }

    public string SalesProductCode { get; set; }

    public int Quantity { get; set; }

    public double? PriceRate { get; set; }

    public string DataKey { get; set; }

    public string CreateOperatorId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UpdateOperatorId { get; set; }

    public DateTime? UpdateAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Product ParentProduct { get; set; }
}