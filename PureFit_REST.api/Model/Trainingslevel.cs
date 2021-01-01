using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    public partial class Trainingslevel
    {
        public Trainingslevel()
        {
            Kunde = new HashSet<Kunde>();
        }

        [Key]
        [Column(TypeName = "INT")]
        public long tr_levelNr { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string tr_levelname { get; set; }
        [Column(TypeName = "VARCHAR(200)")]
        public string tr_beschreibung { get; set; }

        [InverseProperty("K_TrainingslevelNavigation")]
        public virtual ICollection<Kunde> Kunde { get; set; }
    }
}
