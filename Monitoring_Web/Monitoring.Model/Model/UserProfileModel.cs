using Monitoring_Common.Common;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Monitoring_Common.Enum;

namespace Monitoring.Model.Model
{
    public partial class UserProfileModel
    {
        private string? _groupName;
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public int? GroupId { get; set; }
        public string? GroupName
        {
            get
            {
                if (UserType != null)
                {
                    if (UserType == (int)UserTypeEnum.Admin)
                    {
                        return CommonSetting.FullPermission;
                    }
                    else
                        return _groupName;
                }
                return _groupName;
            }
            set
            {
                _groupName = value;
            }
        }
        public int? UserType { get; set; }
        public string? UserTypeName
        {
            get
            {
                if (UserType == null)
                {
                    return null;
                }
                else
                {
                    return Enum.GetName(typeof(Monitoring_Common.Enum.UserTypeEnum), UserType);
                }
            }
        }
        public int? TotalRows { get; set; }
        public bool? IsDelete { get; set; }
    }
}
