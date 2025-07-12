using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.Interfaces
{

    public interface IPasswordResetService
    {
        Task RequestResetAsync(string email);
        Task ResetPasswordAsync(string token, string newPassword);
    }
}
