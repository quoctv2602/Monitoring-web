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
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<List<GroupModel>> GetGroups(GroupFilterRequestModel filterModel)
        {
            return await _groupRepository.GetGroups(filterModel);
        }

        public async Task<GroupModel?> GetById(int id)
        {
            return await _groupRepository.GetById(id);
        }

        public async Task<int> SaveGroup(GroupModel saveModel)
        {
            return await _groupRepository.SaveGroup(saveModel);
        }
        public async Task<int> ChangeDefault(GroupModel changeModel)
        {
            return await _groupRepository.ChangeDefault(changeModel);
        }
        public async Task<int> Delete(List<GroupModel> deleteModels)
        {
            return await _groupRepository.Delete(deleteModels);
        }
    }
}
