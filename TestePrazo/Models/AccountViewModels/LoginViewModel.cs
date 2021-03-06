﻿using System.ComponentModel.DataAnnotations;

namespace TestePrazo.Models.AccountViewModels
{
    public class LoginViewModel
    {

        [Required]
        [Display(Name = "Usuario")]
        public string Username { get; set; }
        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Lembrar?")]
        public bool RememberMe { get; set; }
    }
}
