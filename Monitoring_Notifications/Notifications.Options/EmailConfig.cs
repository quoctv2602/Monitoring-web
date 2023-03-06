﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Options
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int? SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public bool? EnableSSL { get; set; }
    }
}
