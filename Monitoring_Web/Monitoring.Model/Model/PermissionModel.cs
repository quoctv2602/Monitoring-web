using Monitoring.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monitoring_Common.Enum;

namespace Monitoring.Model.Model
{
    public class PermissionModel
    {
        private bool _isSelected;
        public int PageId { get; set; }
        public string? PageName { get; set; } = null!;
        public List<ActionModel> Actions { get; set; } = null!;
        public bool IsSelected
        {
            get
            {
                if (Actions != null)
                {
                    var isSelectAll = Actions.Any(a => a.IsSelected == null || a.IsSelected == false);
                    return !isSelectAll;
                }
                return _isSelected;
            }
            set { _isSelected = value; }
        }
    }
}
