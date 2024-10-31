using Mapster;

namespace Application.DTOs
{
    public class ReturnOrderDto : GenericEntity
    {
        public ReturnOrderDto()
        {
        }

        public ReturnOrderDto(ReturnOrder ro)
        {
            Id = ro.Id;
            ReturnOrderNo = ro.ReturnOrderNo;
            ShipmentNo = ro.ShipmentNo;
            ReturnDate = ro.ReturnDate;
            Reason = ro.Reason;
            PersonInCharge = ro.PersonInCharge;
            ShipTo = ro.ShipTo;
            ShipDate = ro.ShipDate;
            Status = ro.Status;
            CreateOperatorId = ro.CreateOperatorId;
            CreateAt = ro.CreateAt;
            UpdateOperatorId = ro.UpdateOperatorId;
            UpdateAt = ro.UpdateAt;
            IsDeleted = ro.IsDeleted;
        }

        public ReturnOrderDto(ReturnOrder ro, List<ReturnOrderLine> rol)
        {
            Id = ro.Id;
            ReturnOrderNo = ro.ReturnOrderNo;
            ShipmentNo = ro.ShipmentNo;
            ReturnDate = ro.ReturnDate;
            Reason = ro.Reason;
            PersonInCharge = ro.PersonInCharge;
            ShipTo = ro.ShipTo;
            ShipDate = ro.ShipDate;
            Status = ro.Status;
            CreateOperatorId = ro.CreateOperatorId;
            CreateAt = ro.CreateAt;
            UpdateOperatorId = ro.UpdateOperatorId;
            UpdateAt = ro.UpdateAt;
            IsDeleted = ro.IsDeleted;
            ReturnOrderLines = rol.Adapt<List<ReturnOrderLineDto>>();
        }

        public Guid Id { get; set; }

        public string ReturnOrderNo { get; set; } = string.Empty;

        public string ShipmentNo { get; set; }

        public DateOnly? ReturnDate { get; set; }

        public string Reason { get; set; }

        public string PersonInCharge { get; set; }

        public string ShipTo { get; set; }

        public DateOnly? ShipDate { get; set; }
        public EnumReturnOrderStatus Status { get; set; } = EnumReturnOrderStatus.Open;

        public List<ReturnOrderLineDto> ReturnOrderLines { get; set; } = new();
    }

    public class ReturnOrderLineDto : GenericEntity
    {
        public ReturnOrderLineDto()
        {
        }

        public ReturnOrderLineDto(ReturnOrderLine ol)
        {
            Id = ol.Id;
            ReturnOrderNo = ol.ReturnOrderNo;
            Location = ol.Location;
            Qty = ol.Qty;
            Status = ol.Status;
            CreateOperatorId = ol.CreateOperatorId;
            CreateAt = ol.CreateAt;
            UpdateOperatorId = ol.UpdateOperatorId;
            UpdateAt = ol.UpdateAt;
            IsDeleted = ol.IsDeleted;
        }

        public Guid Id { get; set; }

        public string ReturnOrderNo { get; set; } = string.Empty;

        public string Location { get; set; }

        public double? Qty { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
    }
}
