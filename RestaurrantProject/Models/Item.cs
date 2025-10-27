using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurrantProject.Models
{
    public class Item : BaseEntity
    {
        [Required]
        [StringLength(30)]
        [UniqueName]
        public string Name { get; set; } = null!;

        [StringLength(150)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int DailyOrderCount { get; set; } = 0;


        public DateTime LastOrderDate { get; set; } = DateTime.MinValue;

        public int PreparingTime { get; set; } = 10;


        [Required]
        [ForeignKey("CategoryID")]
        public int CategoryID { get; set; }


        public Category? category { get; set; }
    }
}
