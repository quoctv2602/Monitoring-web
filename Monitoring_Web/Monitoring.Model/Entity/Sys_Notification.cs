using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Notification")]
    public partial class Sys_Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = null!;

        public short? NotificationOption { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdateBy { get; set; }
    }
}
