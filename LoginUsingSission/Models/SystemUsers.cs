using System;
using System.Collections.Generic;

namespace LoginUsingSission.Models
{
    public partial class SystemUsers
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
    }
}
