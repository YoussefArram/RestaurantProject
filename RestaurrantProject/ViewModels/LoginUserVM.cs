using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.ViewModels
{
    public class LoginUserVM
    {
        [Required(ErrorMessage ="*")]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}
