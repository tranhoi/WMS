
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PackingListModel
    {
        /// <summary>
        /// ShipingLineId.
        /// </summary>
        public Guid Id { get; set; }
        public string PackingNo { get; set; }

        public string ShipmentNo { get; set; }

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double ShipmentQty { get; set; } = 0;
        public string LocationName { get; set; }
        public string? Location { get; set; }
        public string Bin { get; set; }
        public string? CreateOperatorId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? UpdateOperatorId { get; set; }
        public DateTime? UpdateAt { get; set; }

        /// <summary>
        /// shipment line status.
        /// </summary>
        public EnumShipmentOrderStatus Status { get; set; } = EnumShipmentOrderStatus.Draft;//shipment line status
        /// <summary>
        /// shipment status.
        /// </summary>
        public EnumShipmentOrderStatus StatusOfShipment { get; set; } = EnumShipmentOrderStatus.Draft;//Shipment status
        public double? PackedQty { get; set; } = 0;
        public DateTime? PackedDate { get; set; }
        public Guid IdShipment { get; set; }
        public string? SaleNo { get; set; }
        public int? TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? WHLocation { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PlanShipDate { get; set; }
        public string? CreateOperatorIdOfShipment { get; set; }
        public DateTime? CreateAtOfShipment { get; set; }
        public string? UpdateOperatorIdOfShipment { get; set; }
        public DateTime? UpdateAtOfShipment { get; set; }
        public string? PersonInCharge { get; set; }
        public string? PersonInChargeName { get; set; }
        public string? ShippingCarrierCode { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Telephone { get; set; }
        public string? TrackingNo { get; set; }
        public string? Email { get; set; }
        public string? BinId { get; set; }
        public string? Address { get; set; }
        public string? PickingNo { get; set; }
        public DateTime? PickedDate { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }

        private double? _remaining = 0;
        public double? Remaining
        {
            get { return ShipmentQty - PackedQty; }
            set
            {
                _remaining = value;
            }
        }
    }
}
