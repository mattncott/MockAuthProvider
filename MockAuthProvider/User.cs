namespace MockAuthProvider
{
    public record User(
        Guid Id,
        string Username,
        string Password,
        string Name,
        string Email,
        string Phone,
        string Role);
}