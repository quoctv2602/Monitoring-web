using System.IO;

namespace HealthMeasurement.Api.Models
{
    public class DiskModel
    {
        public string DriveName { get; set; }
        public string VolumeLabel { get; set; }
        public long TotalSize  { get; set; }
        public long TotalFreeSpace { get; set; }
        public string error { get; set; }
    }
}
