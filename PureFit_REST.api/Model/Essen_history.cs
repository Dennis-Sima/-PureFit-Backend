using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    [Table("Essen-history")]
    public partial class Essen_history
    {
        [Key]
        [Column(TypeName = "INT")]
        public long ES_Nr { get; set; }
        [Required]
        [Column(TypeName = "DATE")]
        public byte[] Es_DATE { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(200)")]
        public string ES_RequestData { get; set; }
        [Column(TypeName = "INT")]
        public long ES_Kunde_Nr { get; set; }

        [ForeignKey(nameof(ES_Kunde_Nr))]
        [InverseProperty(nameof(Kunde.Essen_history))]
        public virtual Kunde ES_Kunde_NrNavigation { get; set; }
    }
}
