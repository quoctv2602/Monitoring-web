using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Model.Entity
{
    [Table("Sys_Notification_Detail")]
    public partial class Sys_Notification_Detail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int NotificationId { get; set; }
        public int? KPI { get; set; }
        public string Emails { get; set; } = null!;

        [StringLength(500)]
        public string NotificationAlias { get; set; } = null!;
    }
}
