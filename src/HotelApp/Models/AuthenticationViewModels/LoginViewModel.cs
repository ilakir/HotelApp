using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HotelApp.Models.AuthenticationViewModels
{
    //Модель регистрации пользователя в приложении
    public class LoginViewModel
    {
        [Required]
        [Display(Name =nameof(UserName))]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name =nameof(Password))]
        public string Password { get; set; }
        [Display(Name ="Remember me?")]
        public bool Remember { get; set; }
    }
}
