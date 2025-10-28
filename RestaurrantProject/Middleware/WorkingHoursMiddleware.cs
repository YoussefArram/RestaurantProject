using System.IO;

namespace RestaurrantProject.Middleware
{
    public class WorkingHoursMiddleware
    {
        private readonly RequestDelegate _next;

        public WorkingHoursMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var path = context.Request.Path.Value?.ToLower();

            if (path != null && path.Contains("/home/closed"))
            {
                await _next(context);
                return;
            }

            var now = DateTime.Now.TimeOfDay;
            var openingTime = new TimeSpan(9, 0, 0);  // 9 AM
            var closingTime = new TimeSpan(23, 0, 0); // 11 PM

            if (now < openingTime || now > closingTime)
            {
                context.Response.Redirect("/Home/Closed");
                return;
            }

            await _next(context);
        }
    }
}
