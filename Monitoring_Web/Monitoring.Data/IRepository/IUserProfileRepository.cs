using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data.IRepository
{
    public interface IUserProfileRepository
    {
        Task<List<UserProfileModel>> GetUsers(UserProfileFilterRequestModel filterModel);
        Task<UserProfileModel?> GetUserProfile(int id);
        Task<int> SaveUserProfile(UserProfileModel saveModel);
        Task<int> DeleteUser(UserProfileModel deleteModel);
    }
}
