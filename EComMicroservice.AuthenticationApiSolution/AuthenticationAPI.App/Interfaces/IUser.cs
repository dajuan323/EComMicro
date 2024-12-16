using AuthenticationAPI.App.DTOs;
using EComMicro.SharedLibrary.Responses;

namespace AuthenticationAPI.App.Interfaces;

public interface IUser
{
    Task<Response> Register(AppUserDTO appUserDTO);
    Task<Response> Login(LoginDTO loginDTO);
    Task<GetUserDTO> GetUser(int userId);
}
