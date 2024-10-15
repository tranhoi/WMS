using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.ShippingBoxs
{
    public class ShippingBoxModel
    {
        public Guid Id { get; set; }
        public string BoxName { get; set; }
        public string BoxType { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double MaxWeight { get; set; }
        public string Status { get; set; }
    }
}
