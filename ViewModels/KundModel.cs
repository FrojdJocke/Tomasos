using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TomasosASP.ViewModels
{
    public class KundModel
    {
        public int KundId { get; set; }
        [Required(ErrorMessage = "Du måste fylla i ditt namn")]
        public string Namn { get; set; }
        [Required(ErrorMessage = "Du måste fylla i en adress")]
        public string Gatuadress { get; set; }
        [Required(ErrorMessage = "Du måste fylla i ett postnummer")]
        public string Postnr { get; set; }
        [Required(ErrorMessage = "Du måste fylla i en postort")]
        public string Postort { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        [Required(ErrorMessage = "Du måste fylla i ett användarnamn")]
        public string AnvandarNamn { get; set; }
        [Required(ErrorMessage = "Du måste fylla i ett lösenord")]
        public string Losenord { get; set; }
    }
}
