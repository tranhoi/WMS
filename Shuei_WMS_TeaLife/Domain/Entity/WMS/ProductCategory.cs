﻿using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS
{
    [Table("ProductCategories")]
    public class ProductCategory : GenericEntity
    {
        [Key]
        public int Id { get; set; }

        public string? CategoryName { get; set; }

        public string? Description { get; set; }
        public string? DataKey { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
