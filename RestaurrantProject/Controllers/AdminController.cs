using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurrantProject.Context;
using RestaurrantProject.Models;
using RestaurrantProject.ViewModels;
using System;

//[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly MyContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(MyContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders.CountAsync(o => o.OrderStatus == RestaurrantProject.Enums.OrderStatus.Pending);
        var totalUsers = await _userManager.Users.CountAsync();
        var totalSales = await _context.Orders.SumAsync(o => o.Total);
        var totalItems = await _context.Items.CountAsync(i => i.IsAvailable);

        var viewModel = new AdminDashboardVM
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            TotalUsers = totalUsers,
            TotalSales = totalSales,
            TotalItems = totalItems
        };

        return View(viewModel);
    }
}
