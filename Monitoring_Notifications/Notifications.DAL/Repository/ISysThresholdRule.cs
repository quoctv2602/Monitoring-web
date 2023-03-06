﻿using Notifications.DAL.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Repository
{
    public interface ISysThresholdRule
    {
        IList<SysThresholdRule> GetSysThresholdRules(int environmentId, string machineName);
    }
}