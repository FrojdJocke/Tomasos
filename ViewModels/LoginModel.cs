using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TomasosASP.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Ange användarnamn")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Ange lösenord")]
        public string Password { get; set; }
    }
}
