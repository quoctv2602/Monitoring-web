using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data.IRepository
{
    public interface IReprocessRepository
    {
        Task<ResponseData> UpdateReprocessStatus(ReprocessModel param, int EnvironmentID);
    }
}
