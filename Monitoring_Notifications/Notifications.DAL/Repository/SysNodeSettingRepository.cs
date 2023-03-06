using Notifications.DAL.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Repository
{
    public class SysNodeSettingRepository : ISysNodeSetting
    {
        public IList<SysNodeSetting> GetSysNodeSettings()
        {
            return new List<SysNodeSetting>();
        }
    }
}
