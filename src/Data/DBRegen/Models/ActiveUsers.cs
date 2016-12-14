using System;
using System.Collections.Generic;

namespace DBRegen.Models
{
    public partial class ActiveUsers
    {
        public long Id { get; set; }
        public string AuthnToken { get; set; }
        public string ContainerName { get; set; }
        public string ResourceName { get; set; }
        public string Sas { get; set; }
    }
}
