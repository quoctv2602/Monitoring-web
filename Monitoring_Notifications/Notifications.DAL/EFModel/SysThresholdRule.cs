﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Notifications.DAL.EFModel
{
    public partial class SysThresholdRule
    {
        public int Id { get; set; }
        public int NodeSetting { get; set; }
        public int EnvironmentId { get; set; }
        public string MachineName { get; set; }
        public int MonitoringType { get; set; }
        public string MonitoringName { get; set; }
        public byte? Condition { get; set; }
        public int? Threshold { get; set; }
        public int? ThresholdCounter { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}