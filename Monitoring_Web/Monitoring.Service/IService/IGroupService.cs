using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.IService
{
    public interface IGroupService
    {
        Task<List<GroupModel>> GetGroups(GroupFilterRequestModel filterModel);
        Task<GroupModel?> GetById(int id);
        Task<int> SaveGroup(GroupModel saveModel);
        Task<int> ChangeDefault(GroupModel changeModel);
        Task<int> Delete(List<GroupModel> deleteModels);
    }
}
