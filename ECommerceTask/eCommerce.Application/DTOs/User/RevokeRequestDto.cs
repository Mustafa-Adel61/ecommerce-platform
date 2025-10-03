using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Application.DTOs.User
{
    public class RevokeRequestDto 
    { 
        public string RefreshToken { get; set; } = string.Empty; 
    }

}
