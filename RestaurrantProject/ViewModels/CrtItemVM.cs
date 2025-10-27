using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurrantProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurrantProject.ViewModels
{
    public class CrtItemVM
    {
        [Required(ErrorMessage ="Name is required")]
        [StringLength(30)]
        [UniqueName]
        
        public string Name { get; set; } = null!;

        [StringLength(150)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, double.MaxValue,ErrorMessage ="Price must more than 0")]
        public decimal Price { get; set; }


        [Required (ErrorMessage ="Category is Required")]
        [ForeignKey("CategoryID")]
        [Display(Name ="Category")]
        public int CategoryID { get; set; }


        public SelectList? categories { get; set; }
    }
}
