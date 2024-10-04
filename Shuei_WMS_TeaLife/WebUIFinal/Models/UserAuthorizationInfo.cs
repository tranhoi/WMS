using Domain.Entity.WMS.Authentication;
using Microsoft.AspNetCore.Identity;

namespace WebUIFinal
{
    public class UserAuthorizationInfo
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string EmailName { get; set; }
        public List<Roles> Roles { get; set; } = new List<Roles>();
    }
}
