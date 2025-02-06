using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockAuthProvider.Controllers;
using MockAuthProvider.Services.Interfaces;
using NSubstitute;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MockAuthProvider.Test.Controllers
{
    public class UserInfoControllerTests
    {
        public readonly User TestUser = new (
            Guid.NewGuid(),
            "UserName",
            "Secret",
            "Test User",
            "test@email.com",
            "phone",
            "User");

        [Test]
        public void Userinfo_AlwaysReturnsSubScope()
        {
            var responseScopes = RunTest();

            var responseSub = Guid.Parse(responseScopes[Claims.Subject].ToString());

            Assert.That(responseSub, Is.Not.EqualTo(Guid.Empty));
            Assert.That(responseSub, Is.EqualTo(TestUser.Id));
        }

        [Test]
        public void Userinfo_ProfileScope_ReturnsProfileInformation()
        {
            var responseScopes = RunTest("profile");

            var responseSub = Guid.Parse(responseScopes[Claims.Subject].ToString());
            var responseUsername = responseScopes[Claims.Username].ToString();
            var responsePrefUsername = responseScopes[Claims.PreferredUsername].ToString();
            var responseName = responseScopes[Claims.Name].ToString();

            Assert.That(responseSub, Is.Not.EqualTo(Guid.Empty));
            Assert.Multiple(() =>
            {
                Assert.That(responseSub, Is.EqualTo(TestUser.Id));
                Assert.That(responseUsername, Is.EqualTo(TestUser.Username));
                Assert.That(responsePrefUsername, Is.EqualTo(TestUser.Username));
                Assert.That(responseName, Is.EqualTo(TestUser.Name));
            });
        }

        [Test]
        public void Userinfo_EmailScope_ReturnsProfileInformation()
        {
            var responseScopes = RunTest("email");

            var responseSub = Guid.Parse(responseScopes[Claims.Subject].ToString());
            var responseEmail = responseScopes[Claims.Email].ToString();

            Assert.That(responseSub, Is.Not.EqualTo(Guid.Empty));
            Assert.Multiple(() =>
            {
                Assert.That(responseSub, Is.EqualTo(TestUser.Id));
                Assert.That(responseEmail, Is.EqualTo(TestUser.Email));
            });
        }

        [Test]
        public void Userinfo_PhoneScope_ReturnsProfileInformation()
        {
            var responseScopes = RunTest("phone");
            var responseSub = Guid.Parse(responseScopes[Claims.Subject].ToString());
            var responsePhone = responseScopes[Claims.PhoneNumber].ToString();

            Assert.That(responseSub, Is.Not.EqualTo(Guid.Empty));
            Assert.Multiple(() =>
            {
                Assert.That(responseSub, Is.EqualTo(TestUser.Id));
                Assert.That(responsePhone, Is.EqualTo(TestUser.Phone));
            });
        }

        [Test]
        public void Userinfo_RoleScope_ReturnsProfileInformation()
        {
            var responseScopes = RunTest("roles");
            var responseSub = Guid.Parse(responseScopes[Claims.Subject].ToString());
            var responseRole = responseScopes[Claims.Role].ToString();

            Assert.That(responseSub, Is.Not.EqualTo(Guid.Empty));
            Assert.Multiple(() =>
            {
                Assert.That(responseSub, Is.EqualTo(TestUser.Id));
                Assert.That(responseRole, Is.EqualTo(TestUser.Role));
            });
        }

        private Dictionary<string, object> RunTest(string? scope = null)
        {
            var clientService = Substitute.For<IClientsService>();
            clientService.GetUser().Returns(TestUser);

            var controller = new TestController(TestUser, clientService, scope);
            var response = controller.Userinfo() as OkObjectResult;

            var responseScopes = (Dictionary<string, object>)response.Value;

            Assert.That(responseScopes, Is.Not.Empty);

            return responseScopes;
        }
    }

    internal class TestController : UserInfoController
    {
        internal TestController(User user, IClientsService clientService, string? scopes = null)
            : base(clientService)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            if (!string.IsNullOrEmpty(scopes))
            {
                claims.Add(new Claim(Claims.Private.Scope, scopes));
            }

            var identity = new ClaimsIdentity(claims, "Basic");
            var userMock = new ClaimsPrincipal(identity);

            var contextMock = new DefaultHttpContext
            {
                User = userMock
            };

            var controllerContextMock = Substitute.For<ControllerContext>();
            controllerContextMock.HttpContext = contextMock;
            ControllerContext = controllerContextMock;
        }
    }
}