using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Options
{
    public static class GlobalOptions
    {
        public static Configs? Configs
        {
            get;
            internal set;
        }
        public static void SetConfigs(Configs configs)
        {
            string caller = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
            if ("Main".Equals(caller, StringComparison.InvariantCultureIgnoreCase))
            {
                Configs = configs;
            }
        }
    }
}
