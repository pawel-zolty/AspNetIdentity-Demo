using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace AspNetIdentitydemo.KontoInsert
{
    public class KIHandler : OAuthHandler<KontoInsertOptions>
    {
        public KIHandler(IOptionsMonitor<KontoInsertOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        // TODO: Abstract this properties override pattern into the base class?
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            var scopeParameter = properties.GetParameter<ICollection<string>>(OAuthChallengeProperties.ScopeKey); // KI
            var scope = scopeParameter != null ? FormatScope(scopeParameter) : FormatScope(); // ki

            var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            queryStrings.Add("response_type", "code");
            queryStrings.Add("client_id", Options.ClientId);
            queryStrings.Add("redirect_uri", redirectUri);
            queryStrings.Add("scope", scope); // KI scope tez nizej dla google

            var state = Options.StateDataFormat.Protect(properties);
            queryStrings.Add("state", state);

            if (properties.Parameters.TryGetValue(KontoInsertDefaults.ContextIdPropertyName, out object contextId) && contextId is string contextIdValue)
            {
                queryStrings.Add(KontoInsertDefaults.ContextIdPropertyName, contextIdValue);
            }

            var authorizationEndpoint = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, queryStrings);
            return authorizationEndpoint;
        }

        //protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        //{
        //    if (properties.Parameters.TryGetValue(KontoInsertDefaults.ContextIdPropertyName, out object contextId) && contextId is string contextIdValue)
        //    {
        //        properties.Items[KontoInsertDefaults.ContextIdPropertyName] = contextIdValue;
        //    }

        //    await base.HandleChallengeAsync(properties);
        //}

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            try
            {
                var tokenString = await GetToken((r) => Backchannel.SendAsync(r, HttpCompletionOption.ResponseHeadersRead, Context.RequestAborted), context);

                var payload = JsonDocument.Parse(tokenString);

                return OAuthTokenResponse.Success(payload);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while retrieving an access token: the remote server returned {ex.Message}.");

                return OAuthTokenResponse.Failed(new Exception("An error occurred while retrieving an access token."));
            }
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            var sessionId = Request.Query["session_id"];

            // Get the user
            using var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint + $"?sessionId={sessionId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            var productInfoHeaderValue = new ProductInfoHeaderValue(new ProductHeaderValue("rysy"));
            request.Headers.UserAgent.Add(productInfoHeaderValue);

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"An error occurred when retrieving KI user information ({response.StatusCode}). Please check if the authentication information is correct and the corresponding Google+ API is enabled.");
            }

            var userDataString = await response.Content.ReadAsStringAsync();
            var payload = JsonDocument.Parse(userDataString);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            var context =
                new OAuthCreatingTicketContext(claimsPrincipal, properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
            context.RunClaimActions();

            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        private async Task<string> GetToken(Func<HttpRequestMessage, Task<HttpResponseMessage>> requestHandler, OAuthCodeExchangeContext context)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint)
                .WithBasicKiAuthentication(Options.ClientId, Options.ClientSecret)
                .WithUserAgent(Options.UserAgent)
                .WithFormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["redirect_uri"] = context.RedirectUri,
                    ["code"] = context.Code
                });

            using HttpResponseMessage response = await requestHandler(request);
            response.HandleErrors();
            return await response.Content.ReadAsStringAsync();
        }
    }
}