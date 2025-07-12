using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Application.DTOs.Users;

namespace Shipkart.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);

    }
}
