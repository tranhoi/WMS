﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.Account
{
    public class UserClaimsResponseDTO {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<GetRoleResponseDTO> Roles { get; set; }
    }
}
