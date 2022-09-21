using Hubbell.EHubb.BACnetAPI.BusinessManager;
using Hubbell.EHubb.BACnetAPI.BusinessManager.Interfaces;
using Hubbell.EHubb.BACnetAPI.Models;
using Hubbell.EHubb.Common.APIComm.DataObjects;
using Hubbell.EHubb.Common.AppConstant;
using Hubbell.EHubb.Common.Security.Utility.Interface;
using Hubbell.EHubb.Common.TokenHandler.DataObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hubbell.EHubb.BACnetAPI.Controllers
{
    [ApiController]
    [Route(RouteTemplate.BACnetApiController)]
    public class BACnetCommandController : ControllerBase
    {
        public BACnetCommandController(IHttpClientFactory httpClientFactory, IBACnetCommandBm bACnetCommandBm, 
            IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            _httpClientFactory = httpClientFactory;
            _bACnetCommandBm = bACnetCommandBm;
            _clientAPITokenMgr = clientAPITokenMgr;
            _hmacSettings = hmacSettings;
            _apiCommSettings = apiCommSettings;
            _bACnetConfigMgr = bACnetConfigMgr;
        }

        private IBACnetCommandBm _bACnetCommandBm;
        private readonly IHttpClientFactory _httpClientFactory;
        private IClientAPITokenMgr _clientAPITokenMgr;
        private IOptions<HmacSetting> _hmacSettings;
        private IOptions<APICommDO> _apiCommSettings;
        private IBACnetConfigMgr _bACnetConfigMgr;

        [HttpPost("GetSystemValue")]
        public async Task<BACnetCommandResponseModel> GetSystemValue(BACnetCommandRequestModel bACnetCommandRequestModel)
        {
            BACnetCommandResponseModel bACnetCommandResponseModel = await _bACnetCommandBm.GetSystemValue(bACnetCommandRequestModel, 
                _httpClientFactory, _clientAPITokenMgr, _hmacSettings, _apiCommSettings, _bACnetConfigMgr);
            bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
            return bACnetCommandResponseModel;
        }

        [HttpPost("SetSystemValue")]
        public async Task<BACnetCommandResponseModel> SetSystemValue(BACnetCommandRequestModel bACnetCommandRequestModel)
        {
            BACnetCommandResponseModel bACnetCommandResponseModel = await _bACnetCommandBm.SetSystemValue(bACnetCommandRequestModel,
                _httpClientFactory, _clientAPITokenMgr, _hmacSettings, _apiCommSettings, _bACnetConfigMgr);
            bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
            return bACnetCommandResponseModel;
        }

        [HttpPost("GetZoneList")]
        public async Task<BACnetZoneListResponseModel> GetZoneList(BACnetCommandRequestModel bACnetCommandRequestModel)
        {
            BACnetZoneListResponseModel bACnetZoneListResponseModel = await _bACnetCommandBm.GetZoneList(_httpClientFactory, _clientAPITokenMgr, _hmacSettings, _apiCommSettings);

            bACnetZoneListResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
            return bACnetZoneListResponseModel;
        }

        [HttpPost("GetZoneProperties")]
        public async Task<BACnetZonePropertiesResponseModel> GetZoneProperties(BACnetCommandRequestModel bACnetCommandRequestModel)
        {
            BACnetZonePropertiesResponseModel bACnetZonePropertiesResponseModel = await _bACnetCommandBm.GetZoneProperties(bACnetCommandRequestModel, _httpClientFactory, _clientAPITokenMgr, _hmacSettings, _apiCommSettings, _bACnetConfigMgr);
            bACnetZonePropertiesResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
            return bACnetZonePropertiesResponseModel;
        }

        [HttpPost("GetNxValue")]
        public async Task<BACnetCommandResponseModel> GetNxValue(NxValueRequestModel nxValueRequestModel)
        {
            var result = await _bACnetCommandBm.GetNxValue(nxValueRequestModel, _httpClientFactory, _clientAPITokenMgr, _hmacSettings, _apiCommSettings, _bACnetConfigMgr);
            return result;
        }

        [HttpPost("SetNxValue")]
        public async Task<BACnetCommandResponseModel> SetNxValue(NxValueRequestModel nxValueRequestModel)
        {
            var result = await _bACnetCommandBm.SetNxValue(nxValueRequestModel, _httpClientFactory, _clientAPITokenMgr, _hmacSettings, _apiCommSettings, _bACnetConfigMgr);
            return result;
        }
    }
}