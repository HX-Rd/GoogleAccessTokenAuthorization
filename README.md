# GoogleAccessTokenAuthorization
Google Access Token Authentication for .netcore 2.1

## What does this extension do?
This extension adds Google authorization to Web Api's in dot net 2.1. It does not do a OIDC authentication, it only verifies the google
tokens, and adds claims to the principal from googles tokeninfo endpoint and people endpoint. There are some options you can set to affect 
the behavior of the plugin and these are covered here below. There is also some caching support and will also be documented here below.
There is a Google namespace in [.netcore security](https://github.com/aspnet/Security/tree/dev/src/Microsoft.AspNetCore.Authentication.Google),
but as far as I can see it is used for websites to do a Google authentication. If I'm wrong feel free to point it out in the Issues.

## Who is the extension for
This is by no means a best practise for providing authorization to you apis. The best way would to protect it with jwt tokens and setup
a identity server and use jwt tokens from it to authorize. You could then also sign in with google and get the google access token
and send it down to the api's if you need to access the google apis. This does introduce some overhead and some times you just
want to throw togeater an api fast and stil want some authorization. This might be a case where this extension would be helpful.
If you have anything that is not going to be just a hobby implementation of an api, go with the [Identity Server](https://github.com/IdentityServer/IdentityServer4) route.

## How to use the plugin
Using the plugin is strait forward if you are already familiar with .netcore authentication, it's setup as a authentication scheme.

Here is an simple example ( from Startup.cs )
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME;
    })
    .AddGoogleTokenAuthorization();
}
```
Here is an example using all of the supported options
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME;
    })
    .AddGoogleTokenAuthorization(
        GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME,
        GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME,
        o => 
        {
            o.UseMemoryCache = true;
            o.ClientId = "yourappid.apps.googleusercontent.com";
            o.RequireVerifiedEmail = false;
            o.RequireCorrectClientId = true;
            o.ExpieryWindowSeconds = 20;
            o.OnlyAllowOfflineTokens = true;
            o.RequiredScopes = new List<string>
            {
                "https://www.googleapis.com/auth/plus.me",
                "https://www.googleapis.com/auth/userinfo.email",
                "https://www.googleapis.com/auth/userinfo.profile",
                "https://www.googleapis.com/auth/plus.profile.agerange.read",
                "https://www.googleapis.com/auth/plus.profile.language.read"
            };
        }
    );
}
```
Lets go over what the options mean ( they are all optional ).
### UseMemoryCache
*Default is false*   
Uses the `IMemoryCache`. It is a poor mans version of cache sinse if you have multiple servers, the cache will only work on each individual server.
If this option is set, the `IAuthenticationTicketCache` will use the `AuthenticationTicketMemoryCache` implementation wich is the only
implementation provided with the plugin. You can how ever provide you own.
### ClientId
*Default is null*   
This is the client id of the client that provided the access token. The value from the tokeninfo `aud` value must mach this value *only if*
`RequireCorrectClientId` is set to true.
### RequireCorrectClientId
*Default is false*   
If this option is set to true, the `ClientId ` must mach the tokeninfo `aud` value
### RequiredScopes
*Default is null*   
If this value is null, there are no scope restrictions. If you provide scopes here, the token must provide all of these scopes to be able to access the api
### OnlyAllowOfflineTokens
*Default is false*   
If this value is set to true, only offile ( server side tokens ) are allowed. Corresponds to `access_type` from the tokeninfo endpoint
### ExpieryWindowSeconds
Expire tokens before they really expire, you can set a window where the token is considered expired althogh it stil has some time left. 
Usfull if for example the token expires in one second, but there is some processing in the api and when you eventually call google,
the token is expired.

## Protecting an endpoint
Here is an simple example on how to protect an endpoint
```csharp
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME)]
    public ActionResult<IEnumerable<string>> Get()
    {
        return new string[] { "value1", "value2" };
    }
}
```
### Cache
The included cache `IAuthenticationTicketCache` uses this implementation `AuthenticationTicketMemoryCache` which uses `IMemoryCache`.
You can provide your own implementation of `IAuthenticationTicketCache` and bind it to the DI but to be honest if you need it,
you have propably outgrown this extension and should propably be using Identity server instead.

### Claims
There are alot of claims set on the identity with this extension. They are obtained from google's tokeninfo and people endpoints I will just list theme here   
Note that the <NUBMER> suffix indicades that there could be more than one and each entry will have a number counting from one.
#### From TokenInfo
`azp`   
`aud`   
`sub`   
`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier`   
`exp` Value of this is DateTimeOffset.Now.ToUnixTimeSeconds() plus the expiry seconds from the tokeninfo endpoint minus the `options.ExpieryWindowSeconds`     
`exp_epoc` Value of this is DateTimeOffset.Now.ToUnixTimeSeconds() plus the expiry seconds from the tokeninfo endpoint minus the `options.ExpieryWindowSeconds`   
`http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration` Value of this is DateTimeOffset.Now.ToUnixTimeSeconds() plus the expiry seconds from the tokeninfo endpoint minus the `options.ExpieryWindowSeconds`   
`access_type`   
`email`   
`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress`   
`email_verified`   
`scope` Value of this is a space seperated scope list   

#### From People
`id`   
`kind`   
`etag`   
`objectType`   
`displayName`   
`url`   
`isPlusUser`   
`language`   
`circledByCount`   
`nickname`   
`occupation`   
`skills`   
`birthday`   
`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth`   
`gender`   
`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender`   
`tagline`   
`braggingRights`   
`aboutMe`   
`relationshipStatus`   
`plusOneCount`   
`verified`   
`domain`   
`email<NUMBER>`   
`email<NUBMER>.type`   
`url<NUMBER>`   
`url<NUMBER>.type`   
`url<NUMBER>.label`   
`name`   
`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name`   
`familyName`   
`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname`   
`givenName`   
`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname`   
`middleName`   
`honorificPrefix`   
`honorificSuffix`   
`image.url`   
`image.isDefault`   
`organizations<NUMBER>.department`   
`organizations<NUMBER>.description`   
`organizations<NUMBER>.endDate`   
`organizations<NUMBER>.location`   
`organizations<NUMBER>.name`   
`organizations<NUMBER>.primary`   
`organizations<NUMBER>.startDate`   
`organizations<NUMBER>.title`   
`organizations<NUMBER>.type`   
`placesLived<NUMBER>`   
`placesLived<NUMBER>.primary`   
`cover.layout`   
`cover.coverInfo.leftImageOffset`   
`cover.coverInfo.topImageOffset`   
`cover.coverPhoto.height`   
`cover.coverPhoto.width`   
`cover.coverPhoto.url`   
