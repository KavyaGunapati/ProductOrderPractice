using Models.DTOs;
namespace Interfaces.IManager
{
    public interface IAuthManager
    {
        Task<Result<AuthResponse>> AuthenticateAsync(Register register);
        Task<Result<AuthResponse>> LoginAsync(Login login);
        
    }
}