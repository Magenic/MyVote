using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class MvuserRole
    {
        public MvuserRole()
        {
            Mvuser = new HashSet<Mvuser>();
        }

        public int UserRoleId { get; set; }
        public string UserRoleName { get; set; }

        public virtual ICollection<Mvuser> Mvuser { get; set; }
    }
}
