using AspNetIdentitydemo.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetIdentitydemo.Controllers
{
    public class ExternalController : Controller
    {
        private readonly UserManager<MyCustomUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<MyCustomUser> _claimsPrincipalFactory;

        public ExternalController(UserManager<MyCustomUser> userManager,
            IUserClaimsPrincipalFactory<MyCustomUser> claimsPrincipalFactory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _claimsPrincipalFactory = claimsPrincipalFactory ?? throw new ArgumentNullException(nameof(claimsPrincipalFactory));
        }

        [HttpGet]
        public IActionResult Login(string provider)
        {
            var properties = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("LoginCallback"),
                Items = { { "scheme", provider } }
            };

            return Challenge(properties, provider);
        }

        [HttpGet]
        //[Route("/signin-google")]
        public async Task<IActionResult> LoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            var externalUserId = result.Principal.FindFirstValue("sub")
                ?? result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("Cannot find external id");

            var provider = result.Properties.Items["scheme"];
            var user = await _userManager.FindByLoginAsync(provider, externalUserId);

            if (user == null)
            {
                var email = result.Principal.FindFirstValue("email")
                    ?? result.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new MyCustomUser
                        {
                            UserName = email,
                            Email = email,
                        };

                        await _userManager.CreateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, externalUserId, provider));
                }
            }

            if (user == null) return View("Error");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }
    }
}
