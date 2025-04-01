using EnvaTest.DTO;
using EnvaTest.Result;

namespace EnvaTest.Services.Abstract
{
    public interface IAuthService
    {
        Task<Result<TokenDTO>> LoginAsync(LoginDTO loginDTO);
    }
} 