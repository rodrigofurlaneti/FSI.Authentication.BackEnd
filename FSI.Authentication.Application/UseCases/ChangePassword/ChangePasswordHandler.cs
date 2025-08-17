using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.Exceptions;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;

namespace FSI.Authentication.Application.UseCases.ChangePassword
{
    public sealed record ChangePasswordCommand(string Email, string CurrentPassword, string NewPassword);

    public sealed class ChangePasswordHandler
    {
        private readonly IUserAccountRepository _users;
        private readonly IPasswordHasher _hasher;

        public ChangePasswordHandler(IUserAccountRepository users, IPasswordHasher hasher)
        {
            _users = users;
            _hasher = hasher;
        }

        public async Task Handle(ChangePasswordCommand cmd, CancellationToken ct)
        {
            var emailVo = new FSI.Authentication.Domain.ValueObjects.Email(cmd.Email);
            var user = await _users.GetByEmailAsync(emailVo, ct);
            if (user is null) throw new NotFoundException("Usuário não encontrado.");

            if (!_hasher.Verify(cmd.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedException("Senha atual inválida.");

            if (string.IsNullOrWhiteSpace(cmd.NewPassword) || cmd.NewPassword.Length < 8)
                throw new ValidationAppException("Nova senha precisa ter pelo menos 8 caracteres.");

            var newHash = _hasher.Hash(cmd.NewPassword);
            // Assumindo setter público; se for privado, adapte para um método de domínio.
            user.PasswordHash = newHash;

            await _users.UpdateAsync(user, ct);
        }
    }
}
