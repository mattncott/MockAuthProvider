namespace MockAuthProvider.Contracts
{
    public record AuthorizeRequestContract(
        string ClientId,
        string ClientSecret,
        string RedirectUri,
        string Username,
        string Password,
        bool IsConfidential = false);
}