using System.Runtime.InteropServices;
using Mattncott;
using Microsoft.Extensions.Configuration;
using MockAuthProvider.Contracts;
using MockAuthProvider.Services;

namespace MockAuthProvider.Test;

public class ClientsServiceTests
{
    private readonly IConfiguration _configuration;

    private const string ClientId = "testClient";
    private const string ClientSecret = "testSecret";
    private const string RedirectUri = "http://localhost/redirect";

    private const string TestUser = "user";
    private const string TestUserPass = "secret";

    public ClientsServiceTests()
    {
        _configuration = Configuration();
    }

    [TestCase(ClientId, "incorrect")]
    [TestCase("incorrect", ClientSecret)]
    public void Authorize_InvalidClient_Fails(string clientId, string clientSecret)
    {
        var service = new ClientsService(_configuration);
        var request = new AuthorizeRequestContract(
            clientId,
            clientSecret,
            RedirectUri,
            TestUser,
            TestUserPass);
        
        var result = service.Authorize(request);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.EqualTo(false));
            Assert.That(result.ErrorMessage, Is.EqualTo("Client validation failed"));
        });
    }

    [Test]
    public void Authorize_InvalidRedirectUri_Fails()
    {
        var service = new ClientsService(_configuration);
        var request = new AuthorizeRequestContract(
            ClientId,
            ClientSecret,
            "incorrect",
            TestUser,
            TestUserPass,
            IsConfidential: true);
        
        var result = service.Authorize(request);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.EqualTo(false));
            Assert.That(result.ErrorMessage, Is.EqualTo("Redirect URI does not match"));
        });
    }

    [TestCase(TestUser, "incorrect")]
    [TestCase("incorrect", TestUserPass)]
    public void Authorize_InvalidUserCredentials_Fails(string user, string userPass)
    {
        var service = new ClientsService(_configuration);
        var request = new AuthorizeRequestContract(
            ClientId,
            ClientSecret,
            RedirectUri,
            user,
            userPass);
        
        var result = service.Authorize(request);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.EqualTo(false));
            Assert.That(result.ErrorMessage, Is.EqualTo("Either username or password is incorrect"));
        });
    }

    [Test]
    public void Authorize_ConfidentialClient_DoesNotValidateSecret()
    {
        var service = new ClientsService(_configuration);
        var request = new AuthorizeRequestContract(
            ClientId,
            "incorrect",
            RedirectUri,
            TestUser,
            TestUserPass,
            IsConfidential: true);
        
        var result = service.Authorize(request);

        Assert.That(result.Success, Is.EqualTo(true));
    }

    [Test]
    public void Configuration_WithoutAccounts_ThrowsException()
    {
        var inMemorySettings = new List<KeyValuePair<string, string?>> {
            new("Clients:0:ClientId", ClientId),
            new("Clients:0:ClientSecret", ClientSecret),
            new("Clients:0:RedirectUri", RedirectUri),

            new("Jwt:Key", "YourSecretKeyForAuthenticationOfApplication"),
            new("Jwt:Issuer", "localhost"),
            new("Jwt:Audience", "http://localhost"),
        };

        var configuration = Configuration(inMemorySettings);

        Assert.Throws<NullReferenceException>(() => new ClientsService(configuration));
    }

    [Test]
    public void Configuration_WithoutClients_ThrowsException()
    {
        var inMemorySettings = new List<KeyValuePair<string, string?>> {
            new("Accounts:0:Id", "a35e3495-becb-4ad1-9698-6a9c8a5e0ece"),
            new("Accounts:0:Username", TestUser),
            new("Accounts:0:Password", TestUserPass),
            new("Accounts:0:Name", "test"),

            new("Jwt:Key", "YourSecretKeyForAuthenticationOfApplication"),
            new("Jwt:Issuer", "localhost"),
            new("Jwt:Audience", "http://localhost"),
        };

        var configuration = Configuration(inMemorySettings);

        Assert.Throws<NullReferenceException>(() => new ClientsService(configuration));
    }

        [Test]
    public void Configuration_EmptyAccounts_ThrowsException()
    {
        var inMemorySettings = new List<KeyValuePair<string, string?>> {
            new("Accounts", "test"),

            new("Clients:0:ClientId", ClientId),
            new("Clients:0:ClientSecret", ClientSecret),
            new("Clients:0:RedirectUri", RedirectUri),

            new("Jwt:Key", "YourSecretKeyForAuthenticationOfApplication"),
            new("Jwt:Issuer", "localhost"),
            new("Jwt:Audience", "http://localhost"),
        };

        var configuration = Configuration(inMemorySettings);

        Assert.Throws<NullReferenceException>(() => new ClientsService(configuration));
    }

    [Test]
    public void Configuration_EmptyClients_ThrowsException()
    {
        var inMemorySettings = new List<KeyValuePair<string, string?>> {
            new("Accounts:0:Id", "a35e3495-becb-4ad1-9698-6a9c8a5e0ece"),
            new("Accounts:0:Username", TestUser),
            new("Accounts:0:Password", TestUserPass),
            new("Accounts:0:Name", "test"),

            new("Clients", "test"),

            new("Jwt:Key", "YourSecretKeyForAuthenticationOfApplication"),
            new("Jwt:Issuer", "localhost"),
            new("Jwt:Audience", "http://localhost"),
        };

        var configuration = Configuration(inMemorySettings);

        Assert.Throws<NullReferenceException>(() => new ClientsService(configuration));
    }

        [Test]
    public void Configuration_ValidInput_ReturnsCorrectData()
    {
        var service = new ClientsService(_configuration);
        var request = new AuthorizeRequestContract(
            ClientId,
            ClientSecret,
            RedirectUri,
            TestUser,
            TestUserPass);
        
        var result = service.Authorize(request);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.EqualTo(true));
            Assert.That(result.AccessToken, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.Null);
            Assert.That(result.ExpiresIn, Is.EqualTo(3600));
        });
    }

    private static IConfiguration Configuration(IEnumerable<KeyValuePair<string, string?>> inMemorySettings, bool initialize = true)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        if (initialize)
        {
            ConfigurationHelper.Initialize(configuration);
        }

        return configuration;
    }

    private static IConfiguration Configuration(bool initialize = true)
    {
        var inMemorySettings = new List<KeyValuePair<string, string?>> {
            new("Accounts:0:Id", "a35e3495-becb-4ad1-9698-6a9c8a5e0ece"),
            new("Accounts:0:Username", TestUser),
            new("Accounts:0:Password", TestUserPass),
            new("Accounts:0:Name", "test"),

            new("Clients:0:ClientId", ClientId),
            new("Clients:0:ClientSecret", ClientSecret),
            new("Clients:0:RedirectUri", RedirectUri),

            new("Jwt:Key", "YourSecretKeyForAuthenticationOfApplication"),
            new("Jwt:Issuer", "localhost"),
            new("Jwt:Audience", "http://localhost"),
        };

        return Configuration(inMemorySettings, initialize);
    }
}