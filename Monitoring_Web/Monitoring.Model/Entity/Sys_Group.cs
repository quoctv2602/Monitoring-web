using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Group")]
    public partial class Sys_Group
    {
        [Key]
        public int GroupId { get; set; }
        [StringLength(128)]
        public string? GroupName { get; set; }
        [StringLength(4000)]
        public string? GroupDescription { get; set; }
        [Column(TypeName ="bit")]
        public bool? IsDefault { get; set; }
    }
}
