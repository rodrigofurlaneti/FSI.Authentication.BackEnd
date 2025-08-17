using System;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Application.Mappers
{
    /// <summary>
    /// Mapeamentos utilitários para UserAccount.
    /// Mantido simples para não depender de AutoMapper.
    /// </summary>
    public static class UserMapping
    {
        // Modelo de leitura mínimo (interno à camada Application)
        public sealed record UserReadModel(
            Guid UserId,
            string Email,
            string FirstName,
            string? LastName,
            string ProfileName,
            bool IsActive
        );

        public static UserReadModel ToReadModel(this UserAccount user)
            => new(
                user.UserId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.ProfileName,
                user.IsActive
            );
    }
}
