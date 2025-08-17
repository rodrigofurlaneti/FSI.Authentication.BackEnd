using System.ComponentModel.DataAnnotations;

namespace FSI.Authentication.Presentation.Models.Auth
{
    public sealed class ChangePasswordRequestModel
    {
        // opcional: se nulo, pegar do token
        [EmailAddress] public string? Email { get; set; }
        [Required] public string CurrentPassword { get; set; } = default!;
        [Required, MinLength(6)] public string NewPassword { get; set; } = default!;
    }
}
