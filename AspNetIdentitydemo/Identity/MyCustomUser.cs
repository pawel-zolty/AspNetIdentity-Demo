using Microsoft.AspNetCore.Identity;

namespace AspNetIdentitydemo.Identity
{
    public class MyCustomUser : IdentityUser
    {
        public int OrganizationId { get; set; }
    }
}