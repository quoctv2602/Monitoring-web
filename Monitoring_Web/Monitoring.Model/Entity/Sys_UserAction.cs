using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Sys_UserAction")]
    public partial class Sys_UserAction
    {
        public int GroupId { get; set; }
        public int ActionId { get; set; }
        public int PageId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
    }
}
