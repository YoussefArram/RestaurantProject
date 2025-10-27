using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurrantProject.Models;
using RestaurrantProject.ViewModels;
using System.Security.Claims;

namespace RestaurrantProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
       public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

     
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationVM registrationVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new ApplicationUser()
                {
                    UserName = registrationVM.Username,
                    Email = registrationVM.Email,
                    PasswordHash = registrationVM.Password
                };
               IdentityResult result = await _userManager.CreateAsync(newUser ,registrationVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "Customer");
                    await _signInManager.SignInAsync(newUser, false);
                    return RedirectToAction("GetAll", "Items");
                }
                else
                {
                    foreach(var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }

            return View(registrationVM);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserVM loginUserVM)
        {
            if (ModelState.IsValid)
            {
                //check found
                ApplicationUser applicationUser =  await _userManager.FindByNameAsync(loginUserVM.Name);
                if (applicationUser != null)
                {
                    bool found = await _userManager.CheckPasswordAsync(applicationUser, loginUserVM.Password);
                    if (found)
                    {
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim("Email", applicationUser.Email));
                        await _signInManager.SignInWithClaimsAsync(applicationUser, loginUserVM.RememberMe,claims);
                        
                        return RedirectToAction("Index", "Home");
                    }
                    
                }
                
                    ModelState.AddModelError("", "UserName or Password Invalid");
                


                //create cookie
            }
            return View(loginUserVM);

            
        }

        //[ValidateAntiForgeryToken]
      
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult TestAuth()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                Claim IdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                string id = IdClaim.Value;

                string name = User.Identity.Name;
                return Content(id, name);
            }
            return Content("testauth");
        }
    }
}
 