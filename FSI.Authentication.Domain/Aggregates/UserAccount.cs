using System;
using FSI.Authentication.Domain.ValueObjects;
using FSI.Authentication.Domain.Entities;

namespace FSI.Authentication.Domain.Aggregates
{
    public sealed class UserAccount
    {
        // ========= Identidade =========
        public Guid Id { get; private set; }

        // ========= Dados principais =========
        public Email Email { get; private set; } = default!;
        public PersonName Name { get; private set; } = default!;
        public PasswordHash PasswordHash { get; private set; } = default!;
        /// <summary>Perfil do usuário (ex.: Gerente, Diretor, ...), incluindo permissões.</summary>
        public Profile Profile { get; private set; } = default!;

        // ========= Estado =========
        public bool IsActive { get; private set; }
        public int FailedLoginCount { get; private set; }
        public DateTime? LockoutEndUtc { get; private set; }

        // ========= Auditoria =========
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        // ctor privado para DDD/ORM
        private UserAccount() { }

        private UserAccount(
            Guid id,
            Email email,
            PersonName name,
            PasswordHash passwordHash,
            Profile profile,
            bool isActive,
            int failedLoginCount,
            DateTime? lockoutEndUtc,
            DateTime createdAtUtc,
            DateTime updatedAtUtc)
        {
            Id = id;
            Email = email;
            Name = name;
            PasswordHash = passwordHash;
            Profile = profile;
            IsActive = isActive;
            FailedLoginCount = failedLoginCount;
            LockoutEndUtc = lockoutEndUtc;
            CreatedAtUtc = createdAtUtc;
            UpdatedAtUtc = updatedAtUtc;
        }

        // ========= Factories =========

        /// <summary>Criação de novo usuário (regra de negócio).</summary>
        public static UserAccount Create(Email email, PersonName name, PasswordHash passwordHash, Profile profile)
        {
            var now = DateTime.UtcNow;

            return new UserAccount(
                id: Guid.NewGuid(),
                email: email,
                name: name,
                passwordHash: passwordHash,
                profile: profile,
                isActive: true,
                failedLoginCount: 0,
                lockoutEndUtc: null,
                createdAtUtc: now,
                updatedAtUtc: now
            );
        }

        /// <summary>Reconstituição a partir do banco (sem timestamps informados).</summary>
        public static UserAccount Hydrate(
            Guid id,
            Email email,
            PersonName name,
            PasswordHash passwordHash,
            Profile profile,
            bool isActive,
            int failedLoginCount,
            DateTime? lockoutEndUtc)
        {
            var now = DateTime.UtcNow;

            return new UserAccount(
                id: id,
                email: email,
                name: name,
                passwordHash: passwordHash,
                profile: profile,
                isActive: isActive,
                failedLoginCount: failedLoginCount,
                lockoutEndUtc: lockoutEndUtc,
                createdAtUtc: now,
                updatedAtUtc: now
            );
        }

        /// <summary>Reconstituição a partir do banco (com timestamps).</summary>
        public static UserAccount Hydrate(
            Guid id,
            Email email,
            PersonName name,
            PasswordHash passwordHash,
            Profile profile,
            bool isActive,
            int failedLoginCount,
            DateTime? lockoutEndUtc,
            DateTime createdAtUtc,
            DateTime updatedAtUtc)
        {
            return new UserAccount(
                id: id,
                email: email,
                name: name,
                passwordHash: passwordHash,
                profile: profile,
                isActive: isActive,
                failedLoginCount: failedLoginCount,
                lockoutEndUtc: lockoutEndUtc,
                createdAtUtc: createdAtUtc,
                updatedAtUtc: updatedAtUtc
            );
        }

        // ========= Regras / Comportamentos =========

        public void ChangePassword(PasswordHash newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void ChangeProfile(Profile newProfile)
        {
            Profile = newProfile;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        public void IncrementFailedLogin()
        {
            FailedLoginCount++;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void ResetFailedLogin()
        {
            if (FailedLoginCount != 0)
            {
                FailedLoginCount = 0;
                UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        public void SetLockout(DateTime? untilUtc)
        {
            LockoutEndUtc = untilUtc;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        // ========= Regras de Autenticação =========

        public bool IsLockedOut =>
            LockoutEndUtc.HasValue && LockoutEndUtc.Value > DateTime.UtcNow;

        /// <summary>Usuário pode autenticar? (precisa estar ativo e não bloqueado)</summary>
        public bool CanSignIn()
        {
            if (!IsActive) return false;
            if (IsLockedOut) return false;
            return true;
        }

        /// <summary>Chame após login bem-sucedido.</summary>
        public void OnSuccessfulLogin()
        {
            ResetFailedLogin();
            SetLockout(null);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>Chame após falha de login; aplica lockout se atingir o limite.</summary>
        public void OnFailedLogin(int maxFailedAttempts, TimeSpan lockoutFor)
        {
            IncrementFailedLogin();

            if (FailedLoginCount >= maxFailedAttempts)
            {
                SetLockout(DateTime.UtcNow.Add(lockoutFor));
                // opcional: ResetFailedLogin();
            }

            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
