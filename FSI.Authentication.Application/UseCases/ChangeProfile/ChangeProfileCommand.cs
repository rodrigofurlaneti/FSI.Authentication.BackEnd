namespace FSI.Authentication.Application.UseCases.ChangeProfile
{
    public sealed record ChangeProfileCommand(string Email, string NewProfileName);
}

