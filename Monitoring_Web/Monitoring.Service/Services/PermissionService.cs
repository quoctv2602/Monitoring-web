using Monitoring.Data.IRepository;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<PermissionModel>> GetByGroup(int groupId)
        {
            return await _permissionRepository.GetByGroup(groupId);
        }

        public async Task<int> SavePermission(int groupId, List<PermissionModel> permissions)
        {
            return await _permissionRepository.SavePermission(groupId, permissions);
        }
    }
}
