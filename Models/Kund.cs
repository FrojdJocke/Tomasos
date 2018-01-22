using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TomasosASP.Models
{
    public partial class Kund
    {
        public Kund()
        {
            Bestallning = new HashSet<Bestallning>();
        }

        public int KundID { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ditt namn")]
        [MaxLength(100,ErrorMessage = "Namn får vara max 100 tecken")]
        public string Namn { get; set; }

        [Required(ErrorMessage = "Du måste fylla i en adress")]
        [MaxLength(50,ErrorMessage = "Maxlängd 50 tecken")]
        public string Gatuadress { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett postnummer")]
        [MaxLength(20, ErrorMessage = "Maxlängd 20 tecken")]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "Du måste fylla i en postort")]
        [MaxLength(100, ErrorMessage = "Maxlängd 100 tecken")]
        public string Postort { get; set; }

        [MaxLength(50, ErrorMessage = "Maxlängd 50 tecken")]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "Maxlängd 50 tecken")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett användarnamn")]
        [MinLength(3,ErrorMessage = "Användarnamn måste vara minst 3 tecken")]
        [MaxLength(20, ErrorMessage = "Maxlängd 20 tecken")]
        [DisplayName("Användarnamn")]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett lösenord")]
        [MinLength(6, ErrorMessage = "Lösenord måste vara minst 6 tecken")]
        [MaxLength(20, ErrorMessage = "Maxlängd 20 tecken")]
        public string Losenord { get; set; }

        public int Poang { get; set; }

        public ICollection<Bestallning> Bestallning { get; set; }
    }
}
