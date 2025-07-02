using System.ComponentModel.DataAnnotations;

namespace Survey.DTOs.LoginDto
{
    public class LoginDto
    {

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
