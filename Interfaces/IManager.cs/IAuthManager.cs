using Models.DTOs;
namespace Interfaces.IManager
{
    public interface IAuthManager
    {
        Task<Result<AuthResponse>> RegisterAsync(Register register);
        Task<Result<AuthResponse>> LoginAsync(Login login);
        
    }
}