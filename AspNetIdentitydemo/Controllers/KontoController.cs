using AspNetIdentitydemo.Identity;
using AspNetIdentitydemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
        private readonly MyCustomUserDbContext _dbContext;

        public KontoController(UserManager<MyCustomUser> userManager,
            IUserClaimsPrincipalFactory<MyCustomUser> userClaimsPrincipalFactory,
            MyCustomUserDbContext dbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory ?? throw new ArgumentNullException(nameof(userClaimsPrincipalFactory));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
                    var externalOrganizationId = 1;
                    var organization = await EnsureOrganization(externalOrganizationId);
                    if (organization == null)
                    {
                        throw new Exception("Cannot link to an organization");
                    }

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

        private async Task<Organization> EnsureOrganization(int externalId)
        {
            var organization = await _dbContext.Organizations.Where(o => o.ExternalId == externalId).SingleOrDefaultAsync();

            if (organization == null)
            {
                organization = new Organization
                {
                    Name = "First Demo Org",
                    ExternalId = externalId
                };
                _dbContext.Organizations.Add(organization);
                await _dbContext.SaveChangesAsync();
            }

            return organization;
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
