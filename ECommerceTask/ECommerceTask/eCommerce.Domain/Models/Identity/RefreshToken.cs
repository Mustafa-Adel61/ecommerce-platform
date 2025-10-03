﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Domain.Models.Identity
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;           
        public DateTime Expires { get; set; }                        
        public DateTime Created { get; set; }                        
        public DateTime? Revoked { get; set; }                       
        public string? ReplacedByToken { get; set; }                 
        public string? RemoteIpAddress { get; set; }                 
        public int UserId { get; set; }                              
        public ApplicationUser? User { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
