using Notifications.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.EmailService
{
    public interface IEmailProvider
    {
        void Send(EmailConfig emailConfig, string Tos, string subject, string html);
    }
}
