using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using Microsoft.AspNetCore.Identity.Data;

namespace MagicVilla_Api.Repository.IRepository
{
    public interface IUserRepository
    {
        bool isUnique(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
