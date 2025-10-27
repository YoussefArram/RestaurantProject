using RestaurrantProject.Enums;
using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.ViewModels
{
    public class UpdateOrderVM
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Customer Name")]
        [Required, StringLength(50)]
        public string CustomerName { get; set; } = null!;

        [Display(Name = "Order Type")]
        [Required]
        public OrderType OrderType { get; set; }

        [Display(Name = "Order Status")]
        [Required]
        public OrderStatus OrderStatus { get; set; }

        [Display(Name = "Total Amount")]
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        [Display(Name = "Delivery Address")]
        public string? DeliveryAddress { get; set; }
    }
}