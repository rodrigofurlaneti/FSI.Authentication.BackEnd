using FluentResults;
using Result = FluentResults.Result;
using MediatR;
using FSI.Authentication.Domain.Interfaces;     // IUserAccountService
using FSI.Authentication.Domain.Abstractions;   // IClock
using System;                                   // Convert.ToBase64String, se usou no stub
using System.Text;

namespace FSI.Authentication.Application.UseCases.ChangePassword
{
    public sealed class ChangePasswordHandler
        : IRequestHandler<ChangePasswordCommand, Result<Unit>> // ou Result<bool>, etc.
    {
        private readonly IUserAccountService userService; // exemplo
        private readonly IClock clock;

        public ChangePasswordHandler(IUserAccountService userService, IClock clock)
        {
            this.userService = userService;
            this.clock = clock;
        }

        public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            var user = await userService.GetByEmailAsync(request.Email, ct);
            if (user is null)
                return Result.Fail($"User not found for {request.Email}");

            var newHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.NewPassword));
            user.ChangePassword(newHash, clock);
            await userService.SaveAsync(user, ct);
            return Result.Ok(Unit.Value);
        }
    }
}
