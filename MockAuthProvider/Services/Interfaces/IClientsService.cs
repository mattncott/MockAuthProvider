using MockAuthProvider.Contracts;

namespace MockAuthProvider.Services.Interfaces
{
    public interface IClientsService
    {
        AuthorizeResponseContract Authorize(AuthorizeRequestContract request);
    }
}