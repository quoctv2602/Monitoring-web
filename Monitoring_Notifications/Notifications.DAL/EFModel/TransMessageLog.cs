﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Notifications.DAL.EFModel
{
    public partial class TransMessageLog
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string EmailBcc { get; set; }
        public bool? IsMailServer { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public int? Priority { get; set; }
        public int? EnvironmentId { get; set; }
        public string MachineName { get; set; }
        public int SendType { get; set; }
    }
}