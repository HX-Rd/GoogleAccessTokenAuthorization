using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HXRd.Google.AccessTokenAuthorization.Models;

namespace HXRd.Google.AccessTokenAuthorization
{
    public class GoogleAccessTokenHandler : AuthenticationHandler<GoogleAccessTokenOptions>
    {
        private GoogleHttpClient _googleClient;
        private IOptionsMonitor<GoogleAccessTokenOptions> _optionsMonitor;
        private IAuthenticationTicketCache _cache;

        public GoogleAccessTokenHandler(IOptionsMonitor<GoogleAccessTokenOptions> optionsMonitor, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, GoogleHttpClient googleClient, IServiceProvider provider) 
            : base(optionsMonitor, logger, encoder, clock)
        {
            _googleClient = googleClient;
            _optionsMonitor = optionsMonitor;
            _cache = provider.GetService<IAuthenticationTicketCache>();
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var options = _optionsMonitor.Get(GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME);
            var headers = Request.Headers;
            var authHeader = headers.FirstOrDefault(h => h.Key == "Authorization").Value.ToString();

            if (String.IsNullOrWhiteSpace(authHeader))
            {
                return AuthenticateResult.Fail("Authentication header is null");
            }
            var token = authHeader.Split(' ')[1];
            var cacheTicket = null as AuthenticationTicket;
            if(_cache != null)
                cacheTicket = _cache.GetTicket(token);
            if (cacheTicket != null)
            {
                var exp = long.Parse(cacheTicket.Principal.Claims.FirstOrDefault(c => c.Type == "exp_epoc").Value);
                if (exp < DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    return AuthenticateResult.Fail("Token is expired");
                }
                return AuthenticateResult.Success(cacheTicket);
            }

            var tokenInfo = null as GoogleTokenInfo;
            DateTimeOffset expires;
            var expiresEpoc = null as long?;
            try
            {
                tokenInfo = await _googleClient.GetTokenInfo(token);
                expires = DateTimeOffset.Now.AddSeconds(tokenInfo.ExpiresInSeconds - options.ExpieryWindowSeconds);
                expiresEpoc = expires.ToUnixTimeSeconds();
                if (expiresEpoc < DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    return AuthenticateResult.Fail("Token is expired");
                }
                if (options.OnlyAllowOfflineTokens && tokenInfo.AccessType != "offline")
                {
                    return AuthenticateResult.Fail("AccessToken does not have offline access type");
                }
                if (options.RequireCorrectClientId && tokenInfo.Aud != options.ClientId)
                {
                    return AuthenticateResult.Fail("Audience in the token does not match the ClientId in the options");
                }
                if (options.RequiredScopes != null)
                {
                    var scopes = tokenInfo.Scopes.Split(' ');
                    if (!(scopes.Intersect(options.RequiredScopes).Count() == options.RequiredScopes.Count()))
                    {
                        return AuthenticateResult.Fail("Token does not have all required scopes");
                    }
                }
                if (options.RequireVerifiedEmail == true)
                {
                    if (!tokenInfo.EmailVerified)
                    {
                        return AuthenticateResult.Fail("Only verified emails are allowed");
                    }
                }
            }
            catch (TokenInfoException ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }

            var userInfo = null as GooglePeople;
            try
            {
                userInfo = await _googleClient.GetMe(token);
            }
            catch (UserInfoException ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }

            var identity = MapIdentity(tokenInfo, userInfo, this.Scheme.Name, expiresEpoc);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);
            if(_cache != null)
                _cache.SetTicket(token, ticket, expires);
            return AuthenticateResult.Success(ticket);
        }

        private ClaimsIdentity MapIdentity(GoogleTokenInfo tokenInfo, GooglePeople userInfo, string name, long? expiresEpoc)
        {
            var claimList = new List<Claim>()
            {
                new Claim("azp", tokenInfo.Azp),
                new Claim("aud", tokenInfo.Aud),
                new Claim("sub", tokenInfo.Sub),
                new Claim(ClaimTypes.NameIdentifier, tokenInfo.Sub),
                new Claim("exp", expiresEpoc.ToString()),
                new Claim("exp_epoc", expiresEpoc.ToString()),
                new Claim(ClaimTypes.Expiration, expiresEpoc.ToString()),
                new Claim("access_type", tokenInfo.AccessType),
                new Claim("email", tokenInfo.Email),
                new Claim(ClaimTypes.Email, tokenInfo.Email),
                new Claim("email_verified", tokenInfo.EmailVerified.ToString()),
                new Claim("scope", tokenInfo.Scopes)
            };
            if (userInfo.Id != null) claimList.Add(new Claim("id", userInfo.Id));
            if (userInfo.Kind != null) claimList.Add(new Claim("kind", userInfo.Kind));
            if (userInfo.Etag != null) claimList.Add(new Claim("etag", userInfo.Etag));
            if (userInfo.ObjectType != null) claimList.Add(new Claim("objectType", userInfo.ObjectType));
            if (userInfo.DisplayName != null) claimList.Add(new Claim("displayName", userInfo.DisplayName));
            if (userInfo.Url != null) claimList.Add(new Claim("url", userInfo.Url));
            if (userInfo.IsPlushUser != null) claimList.Add(new Claim("isPlusUser", userInfo.IsPlushUser.ToString()));
            if (userInfo.Language != null) claimList.Add(new Claim("language", userInfo.Language));
            if (userInfo.CircledByCount != null) claimList.Add(new Claim("circledByCount", userInfo.CircledByCount.ToString()));
            if (userInfo.Nickname != null) claimList.Add(new Claim("nickname", userInfo.Nickname));
            if (userInfo.Occupation != null) claimList.Add(new Claim("occupation", userInfo.Occupation));
            if (userInfo.Skills != null) claimList.Add(new Claim("skills", userInfo.Skills));
            if (userInfo.Birthday != null) { claimList.Add(new Claim("birthday", userInfo.Birthday)); claimList.Add(new Claim(ClaimTypes.DateOfBirth, userInfo.Birthday)); }
            if (userInfo.Gender != null) { claimList.Add(new Claim("gender", userInfo.Gender)); claimList.Add(new Claim(ClaimTypes.Gender, userInfo.Gender)); }
            if (userInfo.Tagline != null) claimList.Add(new Claim("tagline", userInfo.Tagline));
            if (userInfo.BraggingRights != null) claimList.Add(new Claim("braggingRights", userInfo.BraggingRights));
            if (userInfo.AboutMe != null) claimList.Add(new Claim("aboutMe", userInfo.AboutMe));
            if (userInfo.RelationshipStatus != null) claimList.Add(new Claim("relationshipStatus", userInfo.RelationshipStatus));
            if (userInfo.PlusOneCount != null) claimList.Add(new Claim("plusOneCount", userInfo.PlusOneCount.ToString()));
            if (userInfo.Verified != null) claimList.Add(new Claim("verified", userInfo.Verified.ToString()));
            if (userInfo.Domain != null) claimList.Add(new Claim("domain", userInfo.Domain));
            if (userInfo.Emails != null)
            {
                for(int i = 0; i < userInfo.Emails.Count; ++i)
                {
                    if (userInfo.Emails[i].Value != null) claimList.Add(new Claim($"email{ i + 1 }", userInfo.Emails[i].Value));
                    if (userInfo.Emails[i].Type != null ) claimList.Add(new Claim($"email{ i + 1 }.type", userInfo.Emails[i].Type));
                }
            }
            if (userInfo.Urls != null)
            {
                for (int i = 0; i < userInfo.Urls.Count; ++i)
                {
                    if (userInfo.Urls[i].Value != null) claimList.Add(new Claim($"url{ i + 1 }", userInfo.Urls[i].Value));
                    if (userInfo.Urls[i].Type != null) claimList.Add(new Claim($"url{ i + 1 }.type", userInfo.Urls[i].Type));
                    if (userInfo.Urls[i].Label != null) claimList.Add(new Claim($"url{ i + 1 }.label", userInfo.Urls[i].Label));
                }
            }
            if (userInfo.Name != null)
            {
                if (userInfo.Name.Formatted != null) { claimList.Add(new Claim("name", userInfo.Name.Formatted)); claimList.Add(new Claim(ClaimTypes.Name, userInfo.Name.Formatted)); }
                if (userInfo.Name.FamilyName != null) { claimList.Add(new Claim("familyName", userInfo.Name.FamilyName)); claimList.Add(new Claim(ClaimTypes.Surname, userInfo.Name.FamilyName)); }
                if (userInfo.Name.GivenName != null) { claimList.Add(new Claim("givenName", userInfo.Name.GivenName)); claimList.Add(new Claim(ClaimTypes.GivenName, userInfo.Name.GivenName)); }
                if (userInfo.Name.MiddleName != null) claimList.Add(new Claim("middleName", userInfo.Name.MiddleName));
                if (userInfo.Name.HonorificPrefix != null) claimList.Add(new Claim("honorificPrefix", userInfo.Name.HonorificPrefix));
                if (userInfo.Name.HonorificSuffix != null) claimList.Add(new Claim("honorificSuffix", userInfo.Name.HonorificSuffix));
            }
            if (userInfo.Image != null)
            {
                if (userInfo.Image.Url != null) claimList.Add(new Claim("image.url", userInfo.Image.Url));
                if (userInfo.Image.IsDefault != null) claimList.Add(new Claim("image.url", userInfo.Image.IsDefault.ToString()));
            }
            if (userInfo.Organizations != null)
            {
                for(int i = 0; i < userInfo.Organizations.Count; ++i)
                {
                    if (userInfo.Organizations[i].Department != null) claimList.Add(new Claim($"organizations{ i + 1 }.department", userInfo.Organizations[i].Department));
                    if (userInfo.Organizations[i].Description != null) claimList.Add(new Claim($"organizations{ i + 1 }.description", userInfo.Organizations[i].Description));
                    if (userInfo.Organizations[i].EndDate != null) claimList.Add(new Claim($"organizations{ i + 1 }.endDate", userInfo.Organizations[i].EndDate));
                    if (userInfo.Organizations[i].Location != null) claimList.Add(new Claim($"organizations{ i + 1 }.location", userInfo.Organizations[i].Location));
                    if (userInfo.Organizations[i].Name != null) claimList.Add(new Claim($"organizations{ i + 1 }.name", userInfo.Organizations[i].Name));
                    if (userInfo.Organizations[i].Primary != null) claimList.Add(new Claim($"organizations{ i + 1 }.primary", userInfo.Organizations[i].Primary.ToString()));
                    if (userInfo.Organizations[i].StartDate != null) claimList.Add(new Claim($"organizations{ i + 1 }.startDate", userInfo.Organizations[i].StartDate));
                    if (userInfo.Organizations[i].Title != null) claimList.Add(new Claim($"organizations{ i + 1 }.title", userInfo.Organizations[i].Title));
                    if (userInfo.Organizations[i].Type != null) claimList.Add(new Claim($"organizations{ i + 1 }.type", userInfo.Organizations[i].Type));
                }
            }
            if (userInfo.PlacesLived != null)
            {
                for(int i = 0; i < userInfo.PlacesLived.Count; ++i)
                {
                    if (userInfo.PlacesLived[i].Value != null) claimList.Add(new Claim($"placesLived{ i + 1 }", userInfo.PlacesLived[i].Value));
                    if (userInfo.PlacesLived[i].Primary != null) claimList.Add(new Claim($"placesLived{ i + 1 }", userInfo.PlacesLived[i].Primary.ToString()));
                }
            }
            if (userInfo.Cover != null)
            {
                if (userInfo.Cover.Layout != null) claimList.Add(new Claim("cover.layout", userInfo.Cover.Layout));
                if (userInfo.Cover.CoverInfo != null)
                {
                    if (userInfo.Cover.CoverInfo.LeftImageOffset != null) claimList.Add(new Claim("cover.coverInfo.leftImageOffset", userInfo.Cover.CoverInfo.LeftImageOffset.ToString()));
                    if (userInfo.Cover.CoverInfo.TopImageOffset != null) claimList.Add(new Claim("cover.coverInfo.topImageOffset", userInfo.Cover.CoverInfo.TopImageOffset.ToString()));
                }
                if (userInfo.Cover.CoverPhoto != null)
                {
                    if (userInfo.Cover.CoverPhoto.Height != null) claimList.Add(new Claim("cover.coverPhoto.height", userInfo.Cover.CoverPhoto.Height.ToString()));
                    if (userInfo.Cover.CoverPhoto.Width != null) claimList.Add(new Claim("cover.coverPhoto.width", userInfo.Cover.CoverPhoto.Width.ToString()));
                    if (userInfo.Cover.CoverPhoto.Url != null) claimList.Add(new Claim("cover.coverPhoto.url", userInfo.Cover.CoverPhoto.Url));
                }
            }
            return new ClaimsIdentity(claimList, name);
        }
    }
}
