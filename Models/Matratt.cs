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
        [Required(ErrorMessage = "Får ej vara tomt")]
        [MaxLength(50, ErrorMessage = "Maxlängd 50 tecken")]
        public string MatrattNamn { get; set; }
        [MaxLength(200, ErrorMessage = "Maxlängd 200 tecken")]
        public string Beskrivning { get; set; }
        [Required]
        [Range(1,100000,ErrorMessage = "Felaktigt värde")]
        public int Pris { get; set; }
        public int MatrattTyp { get; set; }

        public MatrattTyp MatrattTypNavigation { get; set; }
        public ICollection<BestallningMatratt> BestallningMatratt { get; set; }
        public ICollection<MatrattProdukt> MatrattProdukt { get; set; }
    }
}
