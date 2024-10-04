using Microsoft.AspNetCore.Identity;

namespace Domain.Entity.WMS.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Localtion { get; set; }
        public string? Status { get; set; }
    }
}
