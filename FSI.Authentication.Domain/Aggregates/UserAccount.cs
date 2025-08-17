using System.Collections.ObjectModel;
using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Domain.Entities;
using FSI.Authentication.Domain.Events;
using FSI.Authentication.Domain.ValueObjects;

namespace FSI.Authentication.Domain.Aggregates
{
    public sealed class UserAccount : AggregateRoot
    {
        public Email Email { get; private set; } = default!;
        public PersonName Name { get; private set; } = default!;
        public PasswordHash PasswordHash { get; private set; } = default!;

        // Perfil único obrigatório
        public Profile Profile { get; private set; } = default!;

        public bool IsActive { get; private set; } = true;
        public int AccessFailedCount { get; private set; }
        public DateTime? LockoutEndUtc { get; private set; }

        private UserAccount() { }

        private UserAccount(Email email, PersonName name, PasswordHash passwordHash, Profile profile)
        {
            Email = email; Name = name; PasswordHash = passwordHash; Profile = profile;
            Raise(new UserRegistered(Id, email.Value, Name.ToString(), profile.Name.Value, CreatedAtUtc));
        }

        public static UserAccount Register(Email email, PersonName name, PasswordHash passwordHash, Profile profile)
            => new(email, name, passwordHash, profile);

        public void ChangePassword(PasswordHash newHash)
        {
            PasswordHash = newHash ?? throw new ArgumentNullException(nameof(newHash));
            UpdatedAtUtc = DateTime.UtcNow;
            Raise(new PasswordChanged(Id, UpdatedAtUtc.Value));
        }

        public void ChangeProfile(Profile newProfile)
        {
            if (newProfile is null) throw new ArgumentNullException(nameof(newProfile));
            var old = Profile.Name.Value;
            Profile = newProfile;
            UpdatedAtUtc = DateTime.UtcNow;
            Raise(new UserProfileChanged(Id, old, newProfile.Name.Value, UpdatedAtUtc.Value));
        }

        public void Activate()
        {
            if (!IsActive) { IsActive = true; UpdatedAtUtc = DateTime.UtcNow; }
        }

        public void Deactivate()
        {
            if (IsActive) { IsActive = false; UpdatedAtUtc = DateTime.UtcNow; }
        }

        public void RegisterFailedLogin(IClock clock, int maxFailedAttempts, TimeSpan lockoutWindow)
        {
            if (clock is null) throw new ArgumentNullException(nameof(clock));
            AccessFailedCount++;
            Raise(new FailedLoginRegistered(Id, AccessFailedCount, clock.UtcNow));
            if (AccessFailedCount >= maxFailedAttempts)
            {
                LockoutEndUtc = clock.UtcNow.Add(lockoutWindow);
                AccessFailedCount = 0;
                Raise(new UserLocked(Id, LockoutEndUtc.Value));
            }
        }

        public void ResetFailures() => AccessFailedCount = 0;
        public bool CanSignIn(IClock clock) => IsActive && (LockoutEndUtc is null || LockoutEndUtc <= clock.UtcNow);
    }

}
