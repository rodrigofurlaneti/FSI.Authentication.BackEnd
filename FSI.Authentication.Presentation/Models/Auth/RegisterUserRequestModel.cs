using System.ComponentModel.DataAnnotations;

namespace FSI.Authentication.Presentation.Models.Auth
{
    public sealed class RegisterUserRequestModel
    {
        [Required, EmailAddress] public string Email { get; set; } = default!;
        [Required] public string FirstName { get; set; } = default!;
        public string? LastName { get; set; }
        [Required, MinLength(6)] public string Password { get; set; } = default!;
        [Required] public string Profile { get; set; } = default!;
    }
}
