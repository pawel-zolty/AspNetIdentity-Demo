using AspNetIdentitydemo.Identity;
using AspNetIdentitydemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetIdentitydemo.Controllers
{
    public class KontoController : Controller
    {
        //private readonly UserManager<MyUser> _userManager;
        //private readonly UserManager<IdentityUser> _userManager;
        private readonly UserManager<MyCustomUser> _userManager;

        public KontoController(UserManager<MyCustomUser> userManager)
        {
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel register)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(register.UserName);

                if (user == null)
                {
                    user = new MyCustomUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = register.UserName,
                        OrganizationId = 1
                    };

                    var result = await _userManager.CreateAsync(user, register.Password);
                }

                return View("Success");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            const string scheme = CookieAuthenticationDefaults.AuthenticationScheme;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(login.UserName);

                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
                {

                    var identity = new ClaimsIdentity(scheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

                    await HttpContext.SignInAsync(scheme, new ClaimsPrincipal(identity));
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View();
        }
    }
}
