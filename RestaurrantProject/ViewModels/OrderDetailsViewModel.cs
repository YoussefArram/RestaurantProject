using RestaurrantProject.Enums;

namespace RestaurrantProject.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        public bool Selected { get; set; } = false;
    }
}
