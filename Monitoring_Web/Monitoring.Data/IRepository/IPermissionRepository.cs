using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data.IRepository
{
    public interface IPermissionRepository
    {
        Task<List<PermissionModel>> GetByGroup(int groupId);
        Task<int> SavePermission(int groupId, List<PermissionModel> permissions);
    }
}
