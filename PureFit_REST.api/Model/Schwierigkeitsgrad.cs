using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    public partial class Schwierigkeitsgrad
    {
        public Schwierigkeitsgrad()
        {
            Fitness_Uebungen = new HashSet<Fitness_Uebungen>();
        }

        [Key]
        [Column(TypeName = "INT")]
        public long S_Nr { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(200)")]
        public string S_Name { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string S_Bezeichnung { get; set; }

        [InverseProperty("FU_SchwierigkeitsgradNavigation")]
        public virtual ICollection<Fitness_Uebungen> Fitness_Uebungen { get; set; }
    }
}
