using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PureFit_REST.api.Model
{
    public partial class Kunde
    {
        public Kunde()
        {
            Essen_history = new HashSet<Essen_history>();
            Fitness_history = new HashSet<Fitness_history>();
            User = new HashSet<User>();
        }

       
        public long? K_Nr { get; set; }
        public string K_Vorname { get; set; }
        public string K_Zuname { get; set; }
        public string K_Geschlecht { get; set; }
        public byte[] K_Groesse { get; set; }
        public byte[] K_Gewicht { get; set; }
        public byte[] K_GebDatum { get; set; }
        public string K_Email { get; set; }
        public string K_TelefonNr { get; set; }
        public long K_Trainingslevel { get; set; }

        public virtual Trainingslevel K_TrainingslevelNavigation { get; set; }
        public virtual ICollection<Essen_history> Essen_history { get; set; }
        public virtual ICollection<Fitness_history> Fitness_history { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
