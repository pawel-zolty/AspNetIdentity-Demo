using AspNetIdentitydemo.Identity;
using AspNetIdentitydemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetIdentitydemo.Controllers
{
    public class KontoController : Controller
    {
        private readonly UserManager<MyUser> _userManager;

        public KontoController(UserManager<MyUser> userManager)
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
                    user = new MyUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = register.UserName
                    };

                    var result = await _userManager.CreateAsync(user, register.Password);
                }

                return View("Success");
            }

            return View();
        }
    }
}
