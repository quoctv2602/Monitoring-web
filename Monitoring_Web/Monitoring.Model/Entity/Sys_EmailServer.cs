using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Monitoring.Model.Entity
{
    [Table("Sys_EmailServer")]
    public partial class Sys_EmailServer
    {
        [Key]
        public int ID { get; set; }
        [StringLength(250)]
        public string FromEmail { get; set; } = null!;
        [StringLength(250)]
        public string SmtpServer { get; set; } = null!;
        public int? Port { get; set; }
        [StringLength(250)]
        public string? UserName { get; set; }
        [StringLength(250)]
        public string Password { get; set; } = null!;
        [StringLength(250)]
        public string? DisplayName { get; set; }
        public bool? EnableSSL { get; set; }
        public int EnvironmentId { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Comment { get; set; }
    }
}
