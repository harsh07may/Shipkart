using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.Interfaces
{
    public interface ILoginThrottlingService
    {
        Task<bool> IsLockedOutAsync(string email);
        Task RegisterFailedAttemptAsync(string email);
        Task ResetAttemptsAsync(string email);
    }

}
