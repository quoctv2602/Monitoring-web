using System;
using System.Collections.Generic;

namespace Monitoring.Model.Entity
{
    public partial class TransReprocessLog
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? MachineName { get; set; }
        public int? EnvironmentId { get; set; }
        public int ReprocessType { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public int? Week { get; set; }
        public int? DayofYear { get; set; }
        public int? Date { get; set; }
    }
}
