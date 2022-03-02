using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace AspNetIdentitydemo.KontoInsert
{
    public class KontoInsertOptions : OAuthOptions
    {
        public KontoInsertOptions()
        {
            CallbackPath = new PathString("/signin-ki");
            AuthorizationEndpoint = KontoInsertDefaults.AuthorizationEndpoint;
            TokenEndpoint = KontoInsertDefaults.TokenEndpoint;
            UserInformationEndpoint = KontoInsertDefaults.UserInformationEndpoint;

            UserAgent = "rysy";
            AccountRole = "GratyfikantPortalEmployee";

            ClaimActions.MapJsonKey(JwtClaimTypes.Subject, "id");
            ClaimActions.MapJsonKey(JwtClaimTypes.Email, "login");
            ClaimActions.MapCustomJson(JwtClaimTypes.Name, user => $"{user.GetString("name")} {user.GetString("surname")}".Trim());
            ClaimActions.MapJsonKey(JwtClaimTypes.GivenName, "name");
            ClaimActions.MapJsonKey(JwtClaimTypes.FamilyName, "surname");
            ClaimActions.MapJsonKey(JwtClaimTypes.Picture, "photo");
        }

        public string UserAgent { get; set; }

        public string AccountRole { get; set; }
    }
}
