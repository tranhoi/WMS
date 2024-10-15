using Domain.Entity.WMS;

namespace Application.DTOs
{
    public class WarehousePutAwayLineDto : GenericEntity
    {
        public Guid Id { get; set; }

        public string PutAwayNo { get; set; } = string.Empty;

        public string ProductCode { get; set; }

        public int UnitId { get; set; }

        public double? JournalQty { get; set; }

        public double? TransQty { get; set; }

        public string Bin { get; set; }

        public string LotNo { get; set; }
    }
}
