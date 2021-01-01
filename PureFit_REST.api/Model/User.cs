using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    public partial class User
    {
        public long? U_ID { get; set; }
        public string U_Name { get; set; }
        public string U_Salt { get; set; }
        public string U_Hash { get; set; }
        public string U_Role { get; set; }
        public long? U_Kunde_Nr { get; set; }
        
        public virtual Kunde U_Kunde_NrNavigation { get; set; }
    }
}
