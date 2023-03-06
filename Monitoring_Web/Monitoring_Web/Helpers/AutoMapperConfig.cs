using AutoMapper;
using Monitoring.Model.Model;

namespace Monitoring_Web.Helpers
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<NodeSettingsJson, KPIListExportModel>();
        }
    }
}
