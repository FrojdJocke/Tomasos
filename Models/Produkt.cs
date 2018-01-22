using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomasosASP.Models
{
    public partial class Produkt
    {
        public Produkt()
        {
            MatrattProdukt = new HashSet<MatrattProdukt>();
        }

        public int ProduktId { get; set; }
        [Required(ErrorMessage = "Produkten måste ha ett namn")]
        [MaxLength(50,ErrorMessage = "Maxlängd 50 tecken")]
        public string ProduktNamn { get; set; }

        public ICollection<MatrattProdukt> MatrattProdukt { get; set; }
    }
}
