using RestaurrantProject.Enums;
using RestaurrantProject.Models;
using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.ViewModels
{
    public class CrtOrderVM
    {


        [Required]
        public string UserID { get; set; } = null!;

        [Required]
        public string CustomerName { get; set; } = null!;

        public string? DeliveryAddress { get; set; }

        public int OrderType { get; set; } // enum value

        public List<OrderItem> Items { get; set; } = new();
    }
}
