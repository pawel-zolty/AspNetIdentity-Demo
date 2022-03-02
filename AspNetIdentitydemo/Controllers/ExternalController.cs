using AspNetIdentitydemo.Identity;
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
    public class ExternalController : Controller
    {
        private static string SCHEME = IdentityConstants.ApplicationScheme;
        private static string EXTERNAL_SCHEME = IdentityConstants.ExternalScheme;
        private readonly UserManager<MyCustomUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<MyCustomUser> _claimsPrincipalFactory;
        private readonly MyCustomUserDbContext _dbContext;

        public ExternalController(UserManager<MyCustomUser> userManager,
            IUserClaimsPrincipalFactory<MyCustomUser> claimsPrincipalFactory,
            MyCustomUserDbContext dbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _claimsPrincipalFactory = claimsPrincipalFactory ?? throw new ArgumentNullException(nameof(claimsPrincipalFactory));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
        public async Task<IActionResult> LoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(EXTERNAL_SCHEME);
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

                    var externalOrganizationId = 2;
                    var organization = await EnsureOrganization(externalOrganizationId);

                    if (user == null)
                    {
                        user = new MyCustomUser
                        {
                            UserName = email,
                            Email = email,
                            OrganizationId = organization.Id
                        };

                        await _userManager.CreateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, externalUserId, provider));
                }
            }

            if (user == null) return View("Error");

            await HttpContext.SignOutAsync(EXTERNAL_SCHEME);
            var claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
            await HttpContext.SignInAsync(SCHEME, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        private async Task<Organization> EnsureOrganization(int externalId)
        {
            var organization = await _dbContext.Organizations.Where(o => o.ExternalId == externalId).SingleOrDefaultAsync();

            if (organization == null)
            {
                organization = new Organization
                {
                    Name = "Google Inc.",
                    ExternalId = externalId
                };
                _dbContext.Organizations.Add(organization);
                await _dbContext.SaveChangesAsync();
            }

            return organization;
        }
    }
}
