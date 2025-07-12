using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.Common.Constants
{
    public static class Constants
    {
        public const string PasswordResetSubject = "ShipKart Password Reset";
        public const string OrderConfirmationSubject = "Your ShipKart Order Confirmation";
        public const int PasswordResetTokenExpiryMinutes = 30;
    }
}
