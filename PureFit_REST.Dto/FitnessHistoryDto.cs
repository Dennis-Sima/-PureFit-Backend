using System;
using System.Collections.Generic;
using System.Text;

namespace PureFit_REST.Dto
{
    public class FitnessHistoryDto
    {
        public long KundenNr { get; set; }
        public string Date { get; set; }
        public string Bewertung { get; set; }
        public long UebungsNr { get; set; }
        public string UebungsName { get; set; }
        public string Dauer { get; set; }
        public string Beschreibung { get; set; }
        public long? Wiederholungen { get; set; }
        public string SchwierigkeitsgradName { get; set; }
        public string MuskelName { get; set; }
        public string Kalorien { get; set; }
        public string Image { get; set; }



    }
}
