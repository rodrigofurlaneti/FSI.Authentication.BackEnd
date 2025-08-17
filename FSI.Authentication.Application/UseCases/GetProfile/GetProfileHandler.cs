using FSI.Authentication.Application.DTOs.Shared;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.GetProfile
{
    public sealed class GetProfileHandler
    {
        private readonly IUserAccountRepository repo;
        public GetProfileHandler(IUserAccountRepository repo) => this.repo = repo;

        public async Task<UserProfileDto> HandleAsync(GetProfileQuery query, CancellationToken ct)
        {
            var email = Email.Create(query.Email);
            var user = await repo.GetByEmailAsync(email, ct) ?? throw new NotFoundException("Usuário não encontrado");
            return UserMapping.ToProfileDto(user);
        }
    }
}
