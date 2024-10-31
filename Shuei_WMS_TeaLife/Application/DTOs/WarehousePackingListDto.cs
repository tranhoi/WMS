using Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class WarehousePackingListDto
    {
        #region Packing list
        /// <summary>
        /// ShipmentId.
        /// </summary>
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; }
        public string LocationName { get; set; }
        public string ShippingCarrierCode { get; set; }

        public string ShippingAddress { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PlanShipDate { get; set; }
        public EnumShipmentOrderStatus StatusOfShipment { get; set; } = EnumShipmentOrderStatus.Draft;
        public string? Telephone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? TrackingNo { get; set; }
        public DateTime? PickedDate { get; set; }
        #endregion
        public List<PackingListModel> ShipmentLines { get; set; }
    }
}
