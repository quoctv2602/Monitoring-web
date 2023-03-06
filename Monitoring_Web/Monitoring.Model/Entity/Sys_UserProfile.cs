using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Sys_UserProfile")]
    public partial class Sys_UserProfile
    {
        [Key]
        public int Id { get; set; }
        [StringLength(150)]
        public string? Email { get; set; }
        [Column(TypeName = "bit")]
        public bool? IsDelete { get; set; }
        public int? GroupId { get; set; }
        public int? UserType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
    }
}
