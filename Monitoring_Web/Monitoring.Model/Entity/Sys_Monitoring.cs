using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Monitoring")]
    public partial class Sys_Monitoring
    {
        [Key]
        public int ID { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(50)]
        public string? Unit { get; set; }
        public int? Orders { get; set; }
        public Int16? NodeType { get; set; }
    }
}
