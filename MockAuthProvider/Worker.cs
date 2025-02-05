using Microsoft.EntityFrameworkCore;
using MockAuthProvider.Services.Interfaces;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MockAuthProvider
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IClientsService _clientsService;

        public Worker(
            IServiceProvider serviceProvider,
            IClientsService clientsService)
        {
            _serviceProvider = serviceProvider;
            _clientsService = clientsService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DbContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            var clients = _clientsService.GetAllClients();

            foreach (var client in clients)
            {
                if (await manager.FindByClientIdAsync(client.ClientId) is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = client.ClientId,
                        ClientSecret = client.ClientSecret,
                        ClientType = !string.IsNullOrEmpty(client.ClientSecret) ? ClientTypes.Confidential : ClientTypes.Public,
                        RedirectUris = 
                        {
                            new Uri(client.RedirectUri)
                        },
                        Permissions =
                        {
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Token,
                            Permissions.ResponseTypes.Code,
                            Permissions.GrantTypes.ClientCredentials,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Email
                        }
                    });
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}