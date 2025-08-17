using FSI.Authentication.Domain.Abstractions;

namespace FSI.Authentication.Domain.Aggregates
{
    public sealed class UserAccount
    {
        // ========== Propriedades básicas (espelham dbo.Users) ==========
        public Guid UserId { get; private set; }
        public string Email { get; private set; } = default!;
        public string FirstName { get; private set; } = default!;
        public string? LastName { get; private set; }
        public string PasswordHash { get; private set; } = default!;
        public bool IsActive { get; private set; }
        public int FailedLoginCount { get; private set; }
        public DateTimeOffset? LockoutEndUtc { get; private set; }
        public string ProfileName { get; private set; } = default!;

        // Campos “de domínio” (não precisam existir no DB; ajudam o modelo)
        public DateTimeOffset CreatedAtUtc { get; private set; }
        public DateTimeOffset UpdatedAtUtc { get; private set; }

        // ========== Ctor de fábrica ==========
        public UserAccount(
            Guid userId,
            string email,
            string firstName,
            string? lastName,
            string passwordHash,
            string profileName,
            bool isActive = true,
            int failedLoginCount = 0,
            DateTimeOffset? lockoutEndUtc = null)
        {
            UserId = userId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
            ProfileName = profileName;
            IsActive = isActive;
            FailedLoginCount = failedLoginCount;
            LockoutEndUtc = lockoutEndUtc;

            var now = DateTimeOffset.UtcNow;
            CreatedAtUtc = now;
            UpdatedAtUtc = now;
        }

        // ========== Regras ==========
        public bool CanSignIn(IClock clock, out string? reason)
        {
            if (!IsActive)
            {
                reason = "Conta inativa.";
                return false;
            }

            if (LockoutEndUtc.HasValue && LockoutEndUtc.Value > clock.UtcNow)
            {
                reason = $"Conta bloqueada até {LockoutEndUtc:O}.";
                return false;
            }

            reason = null;
            return true;
        }

        // >>> Sobrecarga pedida pelo seu serviço (1 argumento) <<<
        public bool CanSignIn(IClock clock) => CanSignIn(clock, out _);

        public void RegisterFailedLogin(IClock clock, int lockoutThreshold = 5, TimeSpan? lockoutFor = null)
        {
            FailedLoginCount++;

            if (lockoutFor is null)
                lockoutFor = TimeSpan.FromMinutes(15);

            if (FailedLoginCount >= lockoutThreshold)
            {
                LockoutEndUtc = clock.UtcNow.Add(lockoutFor.Value);
                FailedLoginCount = 0; // zera após aplicar lockout
            }

            UpdatedAtUtc = clock.UtcNow;
        }

        public void ResetFailedLogins(IClock clock)
        {
            FailedLoginCount = 0;
            UpdatedAtUtc = clock.UtcNow;
        }

        public void Unlock(IClock clock)
        {
            LockoutEndUtc = null;
            UpdatedAtUtc = clock.UtcNow;
        }

        public void ChangePassword(string newPasswordHash, IClock clock)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Hash de senha inválido.", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
            UpdatedAtUtc = clock.UtcNow;
        }

        public void ChangeProfile(string newProfileName, IClock clock)
        {
            if (string.IsNullOrWhiteSpace(newProfileName))
                throw new ArgumentException("Perfil inválido.", nameof(newProfileName));

            ProfileName = newProfileName;
            UpdatedAtUtc = clock.UtcNow;
        }

        public void Deactivate(IClock clock)
        {
            IsActive = false;
            UpdatedAtUtc = clock.UtcNow;
        }

        public void Activate(IClock clock)
        {
            IsActive = true;
            UpdatedAtUtc = clock.UtcNow;
        }
    }
}
