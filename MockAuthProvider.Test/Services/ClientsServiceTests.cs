using Mattncott;
using Microsoft.Extensions.Configuration;
using MockAuthProvider.Services;

namespace MockAuthProvider.Test.Services
{
    public class ClientsServiceTests
    {
        private const string ClientId = "testClient";
        private const string ClientSecret = "testSecret";
        private const string RedirectUri = "http://localhost/redirect";

        private static readonly Guid TestUserId = Guid.Parse("2abbd92d-d80e-415b-be12-ff9524ef086c");
        private const string TestUser = "user";
        private const string TestUserPass = "secret";
        private const string TestUserName = "Matt";
        private const string TestUserPhone = "07";
        private const string TestUserEmail = "email@email.com";
        private const string TestUserRole = "user";

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
                new("Account:Id", "a35e3495-becb-4ad1-9698-6a9c8a5e0ece"),
                new("Account:Username", TestUser),
                new("Account:Password", TestUserPass),
                new("Account:Name", "test"),

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
                new("Account", "test"),

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
                new("Account:Id", "a35e3495-becb-4ad1-9698-6a9c8a5e0ece"),
                new("Account:Username", TestUser),
                new("Account:Password", TestUserPass),
                new("Account:Name", "test"),

                new("Clients", "test"),

                new("Jwt:Key", "YourSecretKeyForAuthenticationOfApplication"),
                new("Jwt:Issuer", "localhost"),
                new("Jwt:Audience", "http://localhost"),
            };

            var configuration = Configuration(inMemorySettings);

            Assert.Throws<NullReferenceException>(() => new ClientsService(configuration));
        }

        [Test]
        public void GetAllClients_ReturnsExpectedClients()
        {
            var configuration = Configuration();
            var service = new ClientsService(configuration);
            var clients = service.GetAllClients();

            var expectedClients = new List<Client>
            {
                new(ClientId, ClientSecret, RedirectUri),
            };

            Assert.Multiple(() =>
            {
                Assert.That(clients.Count(), Is.EqualTo(1));
                Assert.That(clients, Is.EquivalentTo(expectedClients));
            });
        }

        [Test]
        public void GetUser_ReturnsExpectedUser()
        {
            var configuration = Configuration();
            var service = new ClientsService(configuration);
            var user = service.GetUser();

            var expectedUser = new User(
                TestUserId,
                TestUser,
                TestUserPass,
                TestUserName,
                TestUserEmail,
                TestUserPhone,
                TestUserRole);

            Assert.That(user, Is.EqualTo(expectedUser));
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
                new("Account:Id", TestUserId.ToString()),
                new("Account:Username", TestUser),
                new("Account:Password", TestUserPass),
                new("Account:Name", TestUserName),
                new("Account:Phone", TestUserPhone),
                new("Account:Email", TestUserEmail),
                new("Account:Role", TestUserRole),

                new("Clients:0:ClientId", ClientId),
                new("Clients:0:ClientSecret", ClientSecret),
                new("Clients:0:RedirectUri", RedirectUri),
            };

            return Configuration(inMemorySettings, initialize);
        }
    }
}