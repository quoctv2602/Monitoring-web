using Monitoring.Data.IRepository;
using Monitoring.Data.Repository;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        public UserProfileService(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }
        public async Task<List<UserProfileModel>> GetUsers(UserProfileFilterRequestModel filterModel)
        {
            return await _userProfileRepository.GetUsers(filterModel) ;
        }

        public async Task<UserProfileModel?> GetUserProfile(int id)
        {
            return await _userProfileRepository.GetUserProfile(id);
        }

        public async Task<int> Save(UserProfileModel saveModel)
        {
            return await _userProfileRepository.SaveUserProfile(saveModel);
        }

        public async Task<int> Delete(UserProfileModel deleteModel)
        {
            return await _userProfileRepository.DeleteUser(deleteModel);
        }
    }
}
