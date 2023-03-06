using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Monitoring.Model.Entity
{
    [Table("Sys_NodeType")]
    public partial class Sys_NodeType
    {
        [Key]
        public int ID { get; set; }
        [StringLength(128)]
        public string? NodeType { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string? Description { get; set; }
    }
}
