using Notifications.DAL.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Repository
{
    public  class SysThresholdRuleRepository : ISysThresholdRule
    {
        public IList<SysThresholdRule> GetSysThresholdRules(int environmentId, string machineName)
        {
            return new List<SysThresholdRule>();
        }
    }
}
