using Microsoft.IdentityModel.Tokens;
using MockAuthProvider.Contracts;
using MockAuthProvider.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MockAuthProvider.Services
{
    public class ClientsService: IClientsService
    {
        private readonly IEnumerable<Client> _clients;
        private readonly IEnumerable<User> _users;
        private readonly IConfiguration _configuration;

        private const int AccessTokenLifetime = 3600;
        private const int RefreshTokenLifetime = 2592000;

        public ClientsService(IConfiguration config)
        {
            _clients = GetClients(config);
            _users = GetUsers(config);
            _configuration = config;
        }

        public AuthorizeResponseContract Authorize(AuthorizeRequestContract request)
        {
            try 
            {
                if (request.GrantType != "code")
                {
                    throw new NotImplementedException("Only Grant Type code is supported currently");
                }

                var client = GetClientFromRequest(request);
                
                if (client is null)
                {
                    throw new NullReferenceException("Client validation failed");
                }

                if (request.RedirectUri != client.RedirectUri)
                {
                    throw new Exception("Redirect URI does not match");
                }

                var user = GetUserFromRequest(request);

                if (user is null)
                {
                    throw new NullReferenceException("Either username or password is incorrect");
                }

                return ValidateUser(request, user);
            }
            catch (Exception ex)
            {
                return new AuthorizeResponseContract(Success: false, ErrorMessage: ex.Message);
            }
        }

        private AuthorizeResponseContract ValidateUser(AuthorizeRequestContract request, User user)
        {
            // TODO validate password hashes
            if (request.Password != user.Password)
            {
                throw new UnauthorizedAccessException("Either username or password is incorrect");
            }

            return BuildAuthenticationResponse(user);
        }

        private AuthorizeResponseContract BuildAuthenticationResponse(User user)
        {
            var userClaims = GetUserClaims(user);
                
            var accessToken = GenerateAccessToken(userClaims);
            var refreshToken = GenerateRefreshToken(userClaims);

            return new AuthorizeResponseContract(
                Success: true,
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                ExpiresIn: AccessTokenLifetime);
        }

        internal static IEnumerable<Claim> GetUserClaims(User user)
            => new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name)
            };

        internal string GenerateAccessToken(IEnumerable<Claim> claims)
            => GenerateJwtToken(claims, AccessTokenLifetime);

        private string GenerateRefreshToken(IEnumerable<Claim> claims)
            => GenerateJwtToken(claims, RefreshTokenLifetime);

        private string GenerateJwtToken(IEnumerable<Claim>? claims, int lifetime)
        {
            var key = _configuration.GetValue<string>("Jwt:Key");

            if (string.IsNullOrEmpty(key))
            {
                throw new NullReferenceException("Failed to authenticate");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(
              _configuration.GetValue<string>("Jwt:Issuer"),
              _configuration.GetValue<string>("Jwt:Audience"),
              claims,
              expires: DateTime.UtcNow.AddSeconds(lifetime),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(Sectoken);
        }

        private User? GetUserFromRequest(AuthorizeRequestContract request)
            => _users.FirstOrDefault(x => x.Username == request.Username);

        private Client? GetClientFromRequest(AuthorizeRequestContract request)
        {
            if (request.IsConfidential)
            {
                return _clients.FirstOrDefault(x => x.ClientId == request.ClientId);
            }

            return _clients.FirstOrDefault(x => 
                x.ClientId == request.ClientId && x.ClientSecret == request.ClientSecret);
        }

        private static List<Client> GetClients(IConfiguration config)
        {
            var clients = config.GetSection("Clients").Get<List<Client>>();

            if (clients is null || !clients.Any())
            {
                throw new NullReferenceException("Clients is either missing or empty in configuration");
            }

            return clients;
        }

        private static List<User> GetUsers(IConfiguration config)
        {
            var users = config.GetSection("Accounts").Get<List<User>>();

            if (users is null || !users.Any())
            {
                throw new NullReferenceException("Accounts is either missing or empty in configuration");
            }

            return users;
        }
    }
}