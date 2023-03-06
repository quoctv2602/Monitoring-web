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
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<UserLoginModel> OnLoginSuccess(string email)
        {
            return await _accountRepository.OnLoginSuccess(email);
        }
    }
}
