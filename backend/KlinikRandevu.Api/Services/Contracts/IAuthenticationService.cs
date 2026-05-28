using Entities.Data_Transfer_Objects.Authentication;
using Repositories.Contracts;
using Repositories.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> login(LoginDTO loginDTO);
        Task<TokenResponseDTO> RefreshTokenAsync(string refreshToken);
    }
}
