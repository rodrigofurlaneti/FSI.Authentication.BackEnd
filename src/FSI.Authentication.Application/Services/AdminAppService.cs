using FSI.Authentication.Application.DTOs.Geo;
using FSI.Authentication.Application.Interfaces;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Services
{
    public sealed class AdminAppService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminAppService(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
        {
            _adminRepository = adminRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> TestDatabaseAsync(CancellationToken ct)
        {
            bool ret;

             await using (var tx = await _unitOfWork.BeginAsync(ct))
             {
                ret = await _adminRepository.TestDatabaseAsync(ct);
                await tx.CommitAsync();
             }

            return ret;
        }
    }
}
