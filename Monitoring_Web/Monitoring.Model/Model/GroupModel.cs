using Monitoring.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Model.Model
{
    public class GroupModel : Sys_Group
    {
        public List<UserProfileModel>? Members { get; set; }
        public int TotalMembers
        {
            get
            {
                if (Members == null)
                {
                    return 0;
                }
                else
                    return Members.Count;
            }
        }
        public int TotalRows { get; set; }
    }
}
