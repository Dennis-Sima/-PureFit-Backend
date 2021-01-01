using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    [Table("Fitness-Uebungen")]
    public partial class Fitness_Uebungen
    {
        public Fitness_Uebungen()
        {
            Fitness_history = new HashSet<Fitness_history>();
        }

        [Key]
        [Column(TypeName = "INT")]
        public long FU_Nr { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR (200)")]
        public string FU_Name { get; set; }
        [Required]
        [Column(TypeName = "TIME")]
        public byte[] FU_Dauer { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR (200)")]
        public string FU_Beschreibung { get; set; }
        [Column(TypeName = "VARCHAR (200)")]
        public string FU_ImageSorce { get; set; }
        [Column(TypeName = "VARCHAR (200)")]
        public string FU_VideoSorce { get; set; }
        public long? FU_Wiederholungen { get; set; }
        [Column(TypeName = "DECIMAL (5, 2)")]
        public byte[] FU_Kalorien { get; set; }
        [Column(TypeName = "INT")]
        public long FU_Schwierigkeitsgrad { get; set; }
        public long? FU_Muskel_Nr { get; set; }

        [ForeignKey(nameof(FU_Muskel_Nr))]
        [InverseProperty(nameof(Muskelgruppe.Fitness_Uebungen))]
        public virtual Muskelgruppe FU_Muskel_NrNavigation { get; set; }
        [ForeignKey(nameof(FU_Schwierigkeitsgrad))]
        [InverseProperty(nameof(Schwierigkeitsgrad.Fitness_Uebungen))]
        public virtual Schwierigkeitsgrad FU_SchwierigkeitsgradNavigation { get; set; }
        [InverseProperty("FH_Fitness_Uebungen_NrNavigation")]
        public virtual ICollection<Fitness_history> Fitness_history { get; set; }
    }
}
