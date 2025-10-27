using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurrantProject.ViewModels
{
    public class UpdateItemVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(30)]
        public string Name { get; set; } = null!;

        [StringLength(150)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "Category is Required")]
        [ForeignKey("CategoryID")]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }


        public SelectList? categories { get; set; }
    }
}
