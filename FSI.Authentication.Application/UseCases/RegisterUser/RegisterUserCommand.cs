namespace FSI.Authentication.Application.UseCases.RegisterUser
{
    public sealed record RegisterUserCommand(
        string Email,
        string FirstName,
        string? LastName,
        string Password,
        string ProfileName
    );
}
