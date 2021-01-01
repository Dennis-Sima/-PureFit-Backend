using System;
using System.Collections.Generic;
using System.Text;

namespace PureFit_REST.Dto
{
    public class UserKundenDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Vorname { get; set; }
        public string Zuname { get; set; }
        public string Geschlecht { get; set; }
        public string Groesse { get; set; }
        public string Gewicht { get; set; }
        public string GebDatum { get; set; }
        public string Email { get; set; }
        public string TelefonNr { get; set; }
        public string Trainingslevel { get; set; }
    }
}
