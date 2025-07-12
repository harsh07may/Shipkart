using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendEmailWithAttachmentsAsync(string to, string subject, string htmlBody, Dictionary<string, byte[]> attachments);
    }
}
