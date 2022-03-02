﻿using IdentityModel;
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

            //Scope.Add("openid");
            //Scope.Add("profile");
            //Scope.Add("email");

            ClaimActions.MapJsonKey(JwtClaimTypes.Subject, "id");
            ClaimActions.MapJsonKey(JwtClaimTypes.Email, "login");
            ClaimActions.MapCustomJson(JwtClaimTypes.Name, user => $"{user.GetString("name")} {user.GetString("surname")}".Trim());
            ClaimActions.MapJsonKey(JwtClaimTypes.GivenName, "name");
            ClaimActions.MapJsonKey(JwtClaimTypes.FamilyName, "surname");
            ClaimActions.MapJsonKey(JwtClaimTypes.Picture, "photo");
        }

        /// <summary>
        /// access_type. Set to 'offline' to request a refresh token.
        /// </summary>
        public string AccessType { get; set; }

        public string UserAgent { get; set; }

        public string AccountRole { get; set; }
    }
}