using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.ViewModels
{
    public class UpdateCategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(30)]
        public string Name { get; set; } = null!;
    }
}
