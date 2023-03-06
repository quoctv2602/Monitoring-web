using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Environment")]
    public partial class Sys_Environment
    {
        [Key]
        public int ID { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Comment { get; set; }
    }
}
