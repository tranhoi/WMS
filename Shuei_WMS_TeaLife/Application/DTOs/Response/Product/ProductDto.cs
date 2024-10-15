using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Response.Product
{
    public class ProductDto
    {
        [Key] public int Id { get; set; }

        public string? ProductName { get; set; }
        public int? StockAvailableQuantity { get; set; }
        public string? ProductCode { get; set; }
        public string? SupplierName { get; set; }
        public int? ProductStatus { get; set; }
        public string? UnitName { get; set; }
        public int UnitId { get; set; }
        public string? CategoryName { get; set; }
        public string ProductStatusString { get; set; }
    }
}
