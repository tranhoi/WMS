using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Response.Product
{
    public class ProductDto
    {
        [Key] public int Id { get; set; }

        public string? ProductName { get; set; }
        /// <summary>
        /// Quantity from table Product
        /// </summary>
        public int? StockAvailableQuantity { get; set; }
        public string? ProductCode { get; set; }
        public string? SupplierName { get; set; }
        public EnumProductStatus? ProductStatus { get; set; }
        public string? UnitName { get; set; }
        public int UnitId { get; set; }
        public string? CategoryName { get; set; }
        public string ProductStatusString { get; set; }
        public string? JanCode { get; set; }
        
        /// <summary>
        /// Quantity from wh trans
        /// </summary>
        public int StockAvailableQuantityTrans { get; set; }
        /// <summary>
        /// Quantity from wh shipment status OPEN
        /// </summary>
        public int QuantityShipment { get; set; }

    }
}
