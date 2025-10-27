using RestaurrantProject.Context;
using System.ComponentModel.DataAnnotations;

namespace RestaurrantProject.Models
{
    public class UniqueNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return null;
            var NewName = value.ToString();
            //MyContext _context = new MyContext();
            var _context = (MyContext)validationContext.GetService(typeof(MyContext))!;
            var x = _context.Items.FirstOrDefault(x => x.Name == NewName);
            if (x != null)
            {
                return new ValidationResult("Name must be uniqe");
            }
            return ValidationResult.Success;
        }
    }
}
