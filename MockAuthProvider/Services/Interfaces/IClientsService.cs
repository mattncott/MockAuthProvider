// using MockAuthProvider.Contracts;

namespace MockAuthProvider.Services.Interfaces
{
    public interface IClientsService
    {
        IEnumerable<Client> GetAllClients();
        User GetUser();
    }
}