using Hubbell.EHubb.BACnetAPI.BusinessManager;
using Hubbell.EHubb.BACnetAPI.BusinessManager.Interfaces;
using Hubbell.EHubb.Common.Security.Utility;
using Hubbell.EHubb.Common.Security.Utility.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Hubbell.EHubb.BACnetAPI
{
    public static class DependencyInjection
    {
        public static void AddServiceScopes(this IServiceCollection services)
        {
            services.AddScoped<IBACnetCommandBm, BACnetCommandBm>();
            services.AddSingleton<IClientAPITokenMgr, ClientAPITokenMgr>();
            services.AddSingleton<IBACnetConfigMgr, BACnetConfigMgr>();
        }
    }
}
