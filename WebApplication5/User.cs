using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication5
{
    public partial class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public virtual Role Role { get; set; }
    }
}
