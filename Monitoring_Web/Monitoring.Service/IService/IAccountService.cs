using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.IService
{
    public interface IAccountService
    {
        Task<UserLoginModel> OnLoginSuccess(string email);
    }
}
