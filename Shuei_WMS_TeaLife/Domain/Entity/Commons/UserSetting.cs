﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Commons;

public partial class UserSetting
{
    [Key] public string UserId { get; set; }

    public string CurrencyCode { get; set; }

    public int PageLength { get; set; }

    public string DataKey { get; set; }
}