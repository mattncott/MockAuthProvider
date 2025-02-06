using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockAuthProvider.Services.Interfaces;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MockAuthProvider.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly IClientsService _clientsService;

        public UserInfoController(
            IClientsService clientsService)
            => _clientsService = clientsService;

        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo"), Produces("application/json")]
        public IActionResult Userinfo()
        {
            var user = _clientsService.GetUser();

            var claims = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
                [Claims.Subject] = user.Id.ToString()
            };

            if (User.HasScope(Scopes.Profile))
            {
                claims[Claims.Username] = user.Username;
                claims[Claims.PreferredUsername] = user.Username;
                claims[Claims.Name] = user.Name;
            }

            if (User.HasScope(Scopes.Email))
            {
                claims[Claims.Email] = user.Email;
            }

            if (User.HasScope(Scopes.Phone))
            {
                claims[Claims.PhoneNumber] = user.Phone;
            }

            if (User.HasScope(Scopes.Roles))
            {
                claims[Claims.Role] = user.Role;
            }

            // Note: the complete list of standard claims supported by the OpenID Connect specification
            // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

            return Ok(claims);
        }
    }
}