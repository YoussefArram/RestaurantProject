using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.ViewModels
{
    public class RegistrationVM
    {
        public string Username { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
