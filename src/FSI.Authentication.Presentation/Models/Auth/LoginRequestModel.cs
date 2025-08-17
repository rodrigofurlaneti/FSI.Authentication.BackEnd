using System.ComponentModel.DataAnnotations;

namespace FSI.Authentication.Presentation.Models.Auth
{
    public sealed class LoginRequestModel
    {
        [Required, EmailAddress] public string Email { get; set; } = default!;
        [Required] public string Password { get; set; } = default!;
    }
}
