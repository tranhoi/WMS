﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity.Commons;

public partial class UserVendor
{
    [Key] public string UserId { get; set; }

    public string VendorCode { get; set; }

    public string DataKey { get; set; }
}