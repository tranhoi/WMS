﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Commons;

public partial class Currency
{
    [Key] public string CurrencyCode { get; set; }

    public string Country { get; set; }

    public string Description { get; set; }

    public bool IsDisplayCurrency { get; set; }

    public string DataKey { get; set; }

    public string CreateOperatorId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UpdateOperatorId { get; set; }

    public DateTime? UpdateAt { get; set; }

    public bool? IsDeleted { get; set; }
}