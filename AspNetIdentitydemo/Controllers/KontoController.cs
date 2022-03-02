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
        private readonly IUserClaimsPrincipalFactory<MyCustomUser> _userClaimsPrincipalFactory;

        public KontoController(UserManager<MyCustomUser> userManager,
            IUserClaimsPrincipalFactory<MyCustomUser> userClaimsPrincipalFactory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory ?? throw new ArgumentNullException(nameof(userClaimsPrincipalFactory));
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
            //const string scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            string scheme = IdentityConstants.ApplicationScheme;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(login.UserName);

                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
                {
                    var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                    await HttpContext.SignInAsync(scheme, principal);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View();
        }
    }
}
