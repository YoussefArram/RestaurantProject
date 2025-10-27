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




        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CrtOrderVM crtOrderVM)
        {

            if (!ModelState.IsValid)
            {
                var cats = await _context.Categories.ToListAsync();

                return View(crtOrderVM);

            }

            else
            {
                Order NewOrder = new Order()
                {
                    //OrderStatus = crtOrderVM.OrderStatus,                    
                    //OrderType = crtOrderVM.OrderType,
                    //CustomerName = crtOrderVM.CustomerName,                    
                    //Total = crtOrderVM.Total                  
                };
                await _context.Orders.AddAsync(NewOrder);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("GetAll");
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

            var order = await _context.Orders
                .Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.UserID == user.Id && o.OrderStatus == OrderStatus.Pending && o.OrderType == orderType);

            if (order == null)
            {
                order = new Order
                {
                    UserID = user.Id,
                    CustomerName = user.UserName ?? "Unknown",
                    OrderType = orderType,
                    OrderStatus = OrderStatus.Pending,
                    SubTotal = 0,
                    Discount = 0,
                    Tax = 0,
                    Total = 0,
                    DeliveryAddress = deliveryAddress
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }
            else if (orderType == OrderType.Delivery && string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                order.DeliveryAddress = deliveryAddress;
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

            var existingItem = order.orderItems.FirstOrDefault(oi => oi.ItemID == itemID);
            if (existingItem != null)
            {
                existingItem.Quanitity += quanitity;
                existingItem.SubTotal = existingItem.Quanitity * existingItem.UnitPrice;
            }
            else
            {
                order.orderItems.Add(new OrderItem
                {
                    ItemID = item.Id,
                    Quanitity = quanitity,
                    UnitPrice = item.Price,
                    SubTotal = item.Price * quanitity
                });
            }

            order.SubTotal = order.orderItems.Sum(i => i.SubTotal);
            order.Tax = order.SubTotal * 0.085m;
            decimal discount = 0;
            var now = DateTime.Now;

            
            if (now.Hour >= 15 && now.Hour < 17)
            {
                discount += order.SubTotal * 0.20m; 
            }

            
            if (order.SubTotal > 100)
            {
                discount += order.SubTotal * 0.10m; 
            }

            order.Discount = discount;
            order.Total = order.SubTotal + order.Tax - order.Discount;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Added to {orderType} order successfully!";
            return RedirectToAction("GetAll", "Items");
        }



        [HttpPost]
        public async Task<IActionResult> ConfirmOrder()
        {
            var user = await _user.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.UserID == user.Id && o.OrderStatus == OrderStatus.Pending);

            if (order == null)
                return RedirectToAction("Menu", "Item");

            order.OrderStatus = OrderStatus.InProgress;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = order.Id });
        }



        public async Task<IActionResult> MyOrders()
        {
            var user = await _user.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .Include(o => o.orderItems)
                .ThenInclude(oi => oi.item)
                .FirstOrDefaultAsync(o => o.UserID == user.Id);

            return View(order);
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


