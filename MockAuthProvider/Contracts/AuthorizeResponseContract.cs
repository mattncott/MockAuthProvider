namespace MockAuthProvider.Contracts
{
    public record AuthorizeResponseContract(
        bool Success,
        string? AccessToken = null,
        string? RefreshToken = null,
        int? ExpiresIn = null,
        string? ErrorMessage = null
    );
}