using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.WMS.Authentication
{
    public class RefreshTokens
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        [Key] public string RefreshToken { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool? Activated { get; set; } = true;
    }
}
