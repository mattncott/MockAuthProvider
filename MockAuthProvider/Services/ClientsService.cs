using MockAuthProvider.Services.Interfaces;

namespace MockAuthProvider.Services
{
    public class ClientsService: IClientsService
    {
        private readonly IEnumerable<Client> _clients;
        private readonly User _user;

        public ClientsService(IConfiguration config)
        {
            _clients = GetClients(config);
            _user = GetUser(config);
        }

        public IEnumerable<Client> GetAllClients()
            => _clients;

        public User GetUser()
            => _user;

        private static List<Client> GetClients(IConfiguration config)
        {
            var clients = config.GetSection("Clients").Get<List<Client>>();

            if (clients is null || !clients.Any())
            {
                throw new NullReferenceException("Clients is either missing or empty in configuration");
            }

            return clients;
        }

        private static User GetUser(IConfiguration config)
        {
            var user = config.GetSection("Account").Get<User>();

            if (user is null)
            {
                throw new NullReferenceException("Account is either missing or empty in configuration");
            }

            return user;
        }
    }
}