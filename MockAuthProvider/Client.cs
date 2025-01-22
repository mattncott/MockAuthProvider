namespace MockAuthProvider
{
    public record Client(
        string ClientId,
        string ClientSecret,
        string RedirectUri);
}