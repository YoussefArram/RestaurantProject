using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurrantProject.Models
{
    public class OrderItem : BaseEntity
    {
        [Required]
        [ForeignKey("OrderID")]
        public int OrderID { get; set; }

        public Order order { get; set; } = null!;

        [Required]
        [ForeignKey("ItemID")]
        public int ItemID { get; set; } 

        public Item item { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int Quanitity { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public decimal SubTotal { get; set; }




    }
}
