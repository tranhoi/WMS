﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Commons;

public partial class SalesDatum
{
    [Key] public int Id { get; set; }

    public string Type { get; set; }

    public string CompanyId { get; set; }

    public string Channel { get; set; }

    public string OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int OrderYear { get; set; }

    public int OrderQty { get; set; }

    public double CostOfSales { get; set; }

    public double Sales { get; set; }

    public double Profit { get; set; }

    public string Currency { get; set; }

    public string ProductName { get; set; }

    public string ProductCategory { get; set; }

    public string State { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public string CustomerEmail { get; set; }

    public string RepeatPurchase { get; set; }

    public string DataKey { get; set; }
}