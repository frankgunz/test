using Hubbell.EHubb.BACnetAPI.Models;
using Hubbell.EHubb.Common.APIComm.DataObjects;
using Hubbell.EHubb.Common.CommonModels;
using Hubbell.EHubb.Common.ResponseHandler;
using Hubbell.EHubb.Common.Security.Utility.Interface;
using Hubbell.EHubb.Common.TokenHandler.DataObject;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.BusinessManager.Interfaces
{
    public interface IBACnetConfigMgr
    {
        public BACnetSystemConfigResponseModel GetBACnetConfig();

        public Task<BACnetSystemConfigResponseModel> GetBACnetConfig(IHttpClientFactory httpClientFactory,
            IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings);

        public Task<ResponseModel> UpdateBACnetPhysicalNetworkProps(IHttpClientFactory httpClientFactory,
            IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, UpdateBACnetPhysicalNetworkPropsModel updateBACnetPhysicalNetworkPropsModel);

        public void SetBACnetConfig(BACnetSystemConfigResponseModel bNConfigModel);
    }
}
