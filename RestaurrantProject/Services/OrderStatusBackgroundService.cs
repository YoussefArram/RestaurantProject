using Microsoft.EntityFrameworkCore;
using RestaurrantProject.Context;
using RestaurrantProject.Enums;

public class OrderStatusBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderStatusBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<MyContext>();

                // ✅ الخطوة 1: كل Pending بقاله أكتر من 5 دقايق يتحول InProgress
                var pendingOrders = await _context.Orders
                    .Where(o => o.OrderStatus == OrderStatus.Pending
                             && o.CreatedAt <= DateTime.UtcNow.AddMinutes(-5))
                    .ToListAsync();

                foreach (var order in pendingOrders)
                {
                    order.OrderStatus = OrderStatus.InProgress;
                    order.UpdatedAt = DateTime.UtcNow;
                }

                // ✅ الخطوة 2: كل Order InProgress نتحقق إذا كل الـ items خلصت الـ preparing time
                var inProgressOrders = await _context.Orders
                    .Include(o => o.orderItems)
                    .ThenInclude(oi => oi.item)
                    .Where(o => o.OrderStatus == OrderStatus.InProgress)
                    .ToListAsync();

                foreach (var order in inProgressOrders)
                {
                    bool allReady = true;

                    foreach (var orderItem in order.orderItems)
                    {
                        // نحسب الوقت اللي فات من بداية الـ InProgress
                        if (order.UpdatedAt.HasValue)
                        {
                            var elapsed = DateTime.UtcNow - order.UpdatedAt.Value;

                            if (elapsed.TotalMinutes < orderItem.item.PreparingTime)
                            {
                                allReady = false;
                                break;
                            }
                        }
                        else
                        {
                            // لو UpdatedAt لسه null، معناها لسه مبدأش التحضير
                            allReady = false;
                            break;
                        }

                    }

                    if (allReady)
                    {
                        order.OrderStatus = OrderStatus.Completed;
                        order.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
            }

            // ننتظر دقيقة قبل الفحص الجاي (ممكن تزود أو تقلل)
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
