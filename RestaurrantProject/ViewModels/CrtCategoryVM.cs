using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.ViewModels
{
    public class CrtCategoryVM
    {
        [Required(ErrorMessage ="Name is Required")]
        [StringLength(30)]
        public string Name { get; set; } = null!;
    }
}
