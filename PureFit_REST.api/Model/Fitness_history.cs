using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    [Table("Fitness-history")]
    public partial class Fitness_history
    {
        [Key]
        [Column("FH_Fitness-Uebungen_Nr", TypeName = "INT")]
        public long FH_Fitness_Uebungen_Nr { get; set; }
        [Key]
        [Column(TypeName = "INT")]
        public long FH_Kunde_Nr { get; set; }
        [Key]
        [Column(TypeName = "DATETIME")]
        public byte[] FH_Date { get; set; }
        [Required]
        [Column(TypeName = "DECIMAL (3, 2)")]
        public byte[] FH_Bewertung { get; set; }

        [ForeignKey(nameof(FH_Fitness_Uebungen_Nr))]
        [InverseProperty(nameof(Fitness_Uebungen.Fitness_history))]
        public virtual Fitness_Uebungen FH_Fitness_Uebungen_NrNavigation { get; set; }
        [ForeignKey(nameof(FH_Kunde_Nr))]
        [InverseProperty(nameof(Kunde.Fitness_history))]
        public virtual Kunde FH_Kunde_NrNavigation { get; set; }
    }
}
