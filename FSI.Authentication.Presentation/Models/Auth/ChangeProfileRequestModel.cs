using System.ComponentModel.DataAnnotations;

namespace FSI.Authentication.Presentation.Models.Auth
{
    public sealed class ChangeProfileRequestModel
    {
        // opcional: se nulo, pega do token
        [EmailAddress] public string? Email { get; set; }
        [Required] public string NewProfile { get; set; } = default!;
    }
}
