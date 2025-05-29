using System.ComponentModel.DataAnnotations;

namespace VoloLinkApp.Models
{
    public class UserSettingsViewModel
    {
        [Required(ErrorMessage = "Ім'я користувача обов'язкове")]
        [Display(Name = "Ім'я користувача")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Прізвище обов'язкове")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}