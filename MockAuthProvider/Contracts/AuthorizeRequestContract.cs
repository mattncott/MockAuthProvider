namespace MockAuthProvider.Contracts
{
    public record AuthorizeRequestContract(
        string ClientId,
        string ClientSecret,
        string RedirectUri,
        string Username,
        string Password,
        string GrantType = "code",
        bool IsConfidential = false);
}