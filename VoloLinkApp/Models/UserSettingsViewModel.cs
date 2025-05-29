using System.ComponentModel.DataAnnotations;

namespace VoloLinkApp.Models
{
    public class UserSettingsViewModel
    {
        [Required(ErrorMessage = "��'� ����������� ����'������")]
        [Display(Name = "��'� �����������")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "��'� ����'������")]
        [Display(Name = "��'�")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "������� ����'������")]
        [Display(Name = "�������")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}