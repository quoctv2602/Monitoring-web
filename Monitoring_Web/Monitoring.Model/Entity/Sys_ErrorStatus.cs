using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Entity
{
    [Table("Sys_ErrorStatus")]
    public class Sys_ErrorStatus
    {
        [Key]
        public int ErrorStatus { get; set; }

        public string ErrorName { get; set; }

        public string Description { get; set; }

        public int? GroupCode { get; set; }
    }
}
