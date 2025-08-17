namespace FSI.Authentication.Application.UseCases.GetProfile
{
    public sealed record ProfileDto(
        string Email,
        string FirstName,
        string? LastName,
        string ProfileName,
        bool IsActive
    );
}
