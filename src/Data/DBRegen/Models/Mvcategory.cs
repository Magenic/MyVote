using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class Mvcategory
    {
        public Mvcategory()
        {
            Mvpoll = new HashSet<Mvpoll>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Mvpoll> Mvpoll { get; set; }
    }
}
