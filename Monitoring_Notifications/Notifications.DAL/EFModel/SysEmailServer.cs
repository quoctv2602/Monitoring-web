﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Notifications.DAL.EFModel
{
    public partial class SysEmailServer
    {
        public int Id { get; set; }
        public string FromEmail { get; set; }
        public string SmtpServer { get; set; }
        public int? Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public bool? EnableSsl { get; set; }
        public int EnvironmentId { get; set; }
        public string Comment { get; set; }
    }
}