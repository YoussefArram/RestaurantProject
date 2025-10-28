using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurrantProject.Context;
using RestaurrantProject.Enums;
using RestaurrantProject.Models;
using RestaurrantProject.ViewModels;

namespace RestaurrantProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _user;
        private readonly MyContext _context;
        
        public OrderController(MyContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }
        public IActionResult GetAll()
        {
            var x = _context.Orders.ToList();
            return View(x);
        }


        public IActionResult Delete(int id)
        {
            var x = _context.Orders.FirstOrDefault(x => x.Id == id);
            x.IsDeleted = true;
            _context.SaveChangesAsync();
            return RedirectToAction("GetAll");
        }

        public async Task<IActionResult> Update(int id)
        {
            var oldOrder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (oldOrder == null)
                return NotFound();

            var vm = new UpdateOrderVM
            {
                Id = oldOrder.Id,
                CustomerName = oldOrder.CustomerName,
                OrderType = oldOrder.OrderType,
                OrderStatus = oldOrder.OrderStatus,
                Total = oldOrder.Total,
                DeliveryAddress = oldOrder.DeliveryAddress
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateOrderVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (order == null)
                return NotFound();

            order.CustomerName = vm.CustomerName;
            order.OrderType = vm.OrderType;
            order.OrderStatus = vm.OrderStatus;
            order.Total = vm.Total;
            order.DeliveryAddress = vm.DeliveryAddress;
            order.UpdatedAt = DateTime.UtcNow;

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Order #{order.Id} updated successfully!";
            return RedirectToAction("ManageOrders");
        }






        [HttpPost]
        public async Task<IActionResult> AddItemToOrder(int itemID, int quanitity = 1, OrderType orderType = OrderType.DineIn, string? deliveryAddress = null)
        {
            var user = await _user.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            
            if (orderType == OrderType.Delivery && string.IsNullOrWhiteSpace(deliveryAddress))
            {
                TempData["Error"] = "Delivery orders require a delivery address.";
                return RedirectToAction("GetAll", "Items");
            }

            
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemID);
            if (item == null)
                return NotFound();

            
            if (item.LastOrderDate.Date != DateTime.Now.Date)
            {
                item.DailyOrderCount = 0;
                item.IsAvailable = true;
                item.LastOrderDate = DateTime.Now.Date;
            }

            if (!item.IsAvailable)
            {
                TempData["Error"] = $"Sorry, the item '{item.Name}' is currently unavailable.";
                return RedirectToAction("GetAll", "Items");
            }

            
            item.DailyOrderCount += quanitity;
            item.LastOrderDate = DateTime.Now;

            if (item.DailyOrderCount >= 50)
            {
                item.IsAvailable = false;
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.ItemID == itemID);
            if (existingItem != null)
            {
                existingItem.Quantity += quanitity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ItemID = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = quanitity
                });
            }

            
            decimal subTotal = cart.Sum(i => i.SubTotal);
            decimal tax = subTotal * 0.085m;
            decimal discount = 0;
            var now = DateTime.Now;

            
            if (now.Hour >= 15 && now.Hour < 17)
            {
                discount += subTotal * 0.20m;
            }

           
            if (subTotal > 100)
            {
                discount += subTotal * 0.10m;
            }

            decimal total = subTotal + tax - discount;

            
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            TempData["Success"] = $"Added '{item.Name}' to cart successfully! (Subtotal: {subTotal:C}, Total: {total:C})";

            return RedirectToAction("GetAll", "Items");
        }


        public IActionResult Cart()
        {
            var cart =  HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int itemId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var itemToRemove = cart.FirstOrDefault(c => c.ItemID == itemId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["Success"] = $"Removed '{itemToRemove.Name}' from your cart.";
            }
            else
            {
                TempData["Error"] = "Item not found in cart.";
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Your cart has been cleared successfully.";
            return RedirectToAction("Cart");
        }




        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(OrderType orderType = OrderType.DineIn, string? deliveryAddress = null)
        {
            
            var user = await _user.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            
            if (orderType == OrderType.Delivery && string.IsNullOrWhiteSpace(deliveryAddress))
            {
                TempData["Error"] = "Please enter your delivery address before confirming your order.";
                return RedirectToAction("Cart");
            }

            
            var order = new Order
            {
                UserID = user.Id,
                CustomerName = user.UserName ?? "Unknown",
                OrderType = orderType,
                OrderStatus = OrderStatus.Pending,
                SubTotal = cart.Sum(i => i.SubTotal),
                Tax = cart.Sum(i => i.SubTotal) * 0.085m,
                DeliveryAddress = deliveryAddress
            };

            order.Total = order.SubTotal + order.Tax;

            
            foreach (var cartItem in cart)
            {
                order.orderItems.Add(new OrderItem
                {
                    ItemID = cartItem.ItemID,
                    Quanitity = cartItem.Quantity,
                    UnitPrice = cartItem.Price,
                    SubTotal = cartItem.SubTotal
                });
            }

            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Order confirmed successfully!";
            return RedirectToAction("GetAll", "Items");
        }




        public async Task<IActionResult> MyOrders()
        {
            var user = await _user.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Include(o => o.orderItems)
                .ThenInclude(oi => oi.item)
                .Where(o => o.UserID == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        


        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var user = await _user.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserID == user.Id);

            if (order == null)
                return Content("There is no Order");

            
            if (order.OrderStatus != OrderStatus.Pending)
            {
                return Content("You can only cancel pending orders.");
            }

            _context.OrderItems.RemoveRange(order.orderItems);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> ManageOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.orderItems)
                    .ThenInclude(oi => oi.item)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }


        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.orderItems)
                    .ThenInclude(oi => oi.item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }




        [HttpPost]
        public async Task<IActionResult> DeleteOrderAdmin(int id)
        {
            var order = await _context.Orders
                .Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            _context.OrderItems.RemoveRange(order.orderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Order #{id} deleted successfully.";
            return RedirectToAction("ManageOrders");
        }





    }
}


