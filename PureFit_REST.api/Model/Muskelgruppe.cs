using PureFit_REST.api.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    public partial class Muskelgruppe
    {

        public Muskelgruppe()
        {
            Fitness_Uebungen = new HashSet<Fitness_Uebungen>();
        }

        [Key]
        public long M_Nr { get; set; }
        [Column(TypeName = "VARCHAR (200)")]
        public string M_NameMuskel { get; set; }
        [Column(TypeName = "VARCHAR (200)")]
        public string M_körperteilName { get; set; }
        [Column(TypeName = "VARCHAR (200)")]
        public string M_ImageMuskel { get; set; }

        [InverseProperty("FU_Muskel_NrNavigation")]
        public virtual ICollection<Fitness_Uebungen> Fitness_Uebungen { get; set; }
    }
}
