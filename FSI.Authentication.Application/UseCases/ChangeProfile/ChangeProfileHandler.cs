#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

// MediatR para IRequestHandler<,>
using MediatR;

// Result<T> do FluentResults (usaremos o nome totalmente qualificado para evitar conflitos)
using FluentResults;

// Saída reutilizada do caso de uso GetProfile
using FSI.Authentication.Application.UseCases.GetProfile;

// Ajuste o namespace abaixo CONFORME onde está sua interface
using FSI.Authentication.Domain.Interfaces;
// Se sua interface estiver em outro lugar, troque para:
// using FSI.Authentication.Application.Interfaces;

namespace FSI.Authentication.Application.UseCases.ChangeProfile
{
    /// <summary>
    /// Handler do caso de uso de alteração de perfil.
    /// </summary>
    public sealed class ChangeProfileHandler
        : IRequestHandler<ChangeProfileCommand, FluentResults.Result<GetProfileOutput>>
    {
        private readonly IProfileService _profiles;

        public ChangeProfileHandler(IProfileService profiles)
            => _profiles = profiles ?? throw new ArgumentNullException(nameof(profiles));

        public async Task<FluentResults.Result<GetProfileOutput>> Handle(
            ChangeProfileCommand request,
            CancellationToken ct)
        {
            // TODO: ajuste esta chamada conforme o contrato do seu serviço.
            // Exemplos comuns (DESCOMENTE o que se aplicar e remova o restante):

            // 1) Serviço recebe (userId, input)
            // var updated = await _profiles.ChangeAsync(request.UserId, request.Input, ct);

            // 2) Serviço recebe o próprio comando
            // var updated = await _profiles.ChangeAsync(request, ct);

            // 3) Serviço recebe campos isolados (ex.: nome, e-mail)
            // var updated = await _profiles.ChangeAsync(request.UserId, request.Name, request.Email, ct);

            // if (updated is null)
            //     return FluentResults.Result.Fail<GetProfileOutput>("Profile not found");

            // TODO: mapeie a entidade/DTO retornada pelo serviço para o GetProfileOutput
            // var output = new GetProfileOutput
            // {
            //     Id = updated.Id,
            //     Name = updated.Name,
            //     Email = updated.Email,
            //     Roles = updated.Roles,
            //     Claims = updated.Claims
            // };

            // return FluentResults.Result.Ok(output);

            // Stub para manter o projeto compilando até você ligar a lógica real:
            await Task.CompletedTask;
            return FluentResults.Result.Fail<GetProfileOutput>("ChangeProfileHandler não implementado.");
        }
    }
}
