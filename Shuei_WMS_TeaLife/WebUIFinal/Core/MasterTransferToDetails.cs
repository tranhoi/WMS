using Application.DTOs;
using Application.Models;

namespace WebUIFinal.Core
{
    public class MasterTransferToDetails
    {
        public WarehousePackingListDto TransferToPackingDetail { get; set; }

        public WarehousePickingDTO TransferToPickingDetail { get; set; }
    }
}
