using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string lasName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationCode { get; set; }
        public bool isVerified { get; set; }
        public Roles Role { get; set; }
    }
}
