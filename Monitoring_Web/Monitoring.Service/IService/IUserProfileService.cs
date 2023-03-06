using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.IService
{
    public interface IUserProfileService
    {
        Task<List<UserProfileModel>> GetUsers(UserProfileFilterRequestModel filterModel);
        Task<UserProfileModel?> GetUserProfile(int id);
        Task<int> Save(UserProfileModel saveModel);
        Task<int> Delete(UserProfileModel deleteModels);
    }
}
