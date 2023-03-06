using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class SavePermissionModel
    {
        public int GroupId { get; set; }
        public List<PermissionModel> Permissions { get; set; } = null!;
    }
}
