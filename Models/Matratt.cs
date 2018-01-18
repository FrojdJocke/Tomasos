using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomasosASP.Models
{
    public partial class Matratt
    {
        public Matratt()
        {
            BestallningMatratt = new HashSet<BestallningMatratt>();
            MatrattProdukt = new HashSet<MatrattProdukt>();
        }

        public int MatrattId { get; set; }
        [Required]
        public string MatrattNamn { get; set; }
        public string Beskrivning { get; set; }
        [Required]
        public int Pris { get; set; }
        public int MatrattTyp { get; set; }

        public MatrattTyp MatrattTypNavigation { get; set; }
        public ICollection<BestallningMatratt> BestallningMatratt { get; set; }
        public ICollection<MatrattProdukt> MatrattProdukt { get; set; }
    }
}
