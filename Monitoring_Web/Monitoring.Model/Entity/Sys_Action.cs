using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Action")]
    public partial class Sys_Action
    {
        [Key]
        public int ActionId { get; set; }
        [Required]
        public int PageId { get; set; }
        [StringLength(256)]
        public string? ActionName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
    }
}
