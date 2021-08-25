using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TomasosASP.Models
{
    public partial class User : IdentityUser
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        [Required(ErrorMessage = "Du måste fylla i ditt namn")]
        [MaxLength(100,ErrorMessage = "Namn får vara max 100 tecken")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Du måste fylla i en adress")]
        [MaxLength(50,ErrorMessage = "Maxlängd 50 tecken")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett postnummer")]
        [MaxLength(20, ErrorMessage = "Maxlängd 20 tecken")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Du måste fylla i en postort")]
        [MaxLength(100, ErrorMessage = "Maxlängd 100 tecken")]
        public string City { get; set; }

        [MaxLength(50, ErrorMessage = "Maxlängd 50 tecken")]
        public string Telephone { get; set; }

        public int Points { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
