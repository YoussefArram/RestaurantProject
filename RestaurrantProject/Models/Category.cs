using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; } = null!;

        public List<Item>? items { get; set; } = new();
    }
}
