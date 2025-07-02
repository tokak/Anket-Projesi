using System.ComponentModel.DataAnnotations;

namespace Survey.DTOs.UserProfileDto
{
    public class UserProfile
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Adı")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyadı")]
        public string LastName { get; set; }
    }
}
