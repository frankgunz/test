using Hubbell.EHubb.BACnetAPI.Models;
using Hubbell.EHubb.Common.APIComm.DataObjects;
using Hubbell.EHubb.Common.Security.Utility.Interface;
using Hubbell.EHubb.Common.TokenHandler.DataObject;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.BusinessManager.Interfaces
{
    public interface IBACnetCommandBm
    {
        Task<BACnetCommandResponseModel> GetSystemValue(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr);
        Task<BACnetCommandResponseModel> SetSystemValue(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr);
        Task<BACnetZoneListResponseModel> GetZoneList(IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings);
        Task<BACnetZonePropertiesResponseModel> GetZoneProperties(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory _httpClientFactory, IClientAPITokenMgr _clientAPITokenMgr, IOptions<HmacSetting> _hmacSettings, IOptions<APICommDO> _apiCommSettings, IBACnetConfigMgr bACnetConfigMgr);
        Task<BACnetCommandResponseModel> GetNxValue(NxValueRequestModel nxValueRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr);
        Task<BACnetCommandResponseModel> SetNxValue(NxValueRequestModel nxValueRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr);
    }
}
