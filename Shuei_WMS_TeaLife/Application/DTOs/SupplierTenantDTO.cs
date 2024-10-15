using Domain.Entity.WMS;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class SupplierTenantDTO
    {
        public int Id { get; set; } // Có thể là ID từ bảng Supplier
        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public int? TenantId { get; set; }
        public string TenantFullName { get; set; }
        public string ParentDataKey { get; set; }
        public bool? IsHierarchical { get; set; }

        // Thêm các thuộc tính khác từ TenantAuth nếu cần
    }
}