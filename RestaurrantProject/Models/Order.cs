using RestaurrantProject.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurrantProject.Models
{
    public class Order : BaseEntity
    {
        

        [Required]
        public OrderType OrderType { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public string UserID { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        [StringLength(200)]
        public string? DeliveryAddress { get; set; }

        public List<OrderItem> orderItems { get; set; } = new();

        
        
    }
}
