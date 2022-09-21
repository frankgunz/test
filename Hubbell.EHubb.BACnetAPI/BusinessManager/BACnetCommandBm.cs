using Hubbell.EHubb.BACnetAPI.BusinessManager.Interfaces;
using Hubbell.EHubb.BACnetAPI.BusinessManager.Mapper;
using Hubbell.EHubb.BACnetAPI.Models;
using Hubbell.EHubb.Common.APIComm.DataObjects;
using Hubbell.EHubb.Common.Exceptions;
using Hubbell.EHubb.Common.Security.Utility.Interface;
using Hubbell.EHubb.Common.TokenHandler.DataObject;
using Hubbell.EHubb.Device.Infrastructure.Model;
using Hubbell.EHubb.Common.AppConstant;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Hubbell.EHubb.Device.Infrastructure.Model.NxAdapterRequest;
using System.Linq.Expressions;
using Hubbell.EHubb.Device.Infrastructure.Helper;
using Hubbell.EHubb.Common.Extenstion;
using Hubbell.EHubb.Common.Logger;
using Hubbell.EHubb.Common.ResponseHandler;
using Hubbell.EHubb.Common.CommonModels;

namespace Hubbell.EHubb.BACnetAPI.BusinessManager
{
    public class BACnetCommandBm : IBACnetCommandBm
    {
        /// <summary>
        /// GetSystemValue - This API will retrieve a single BACnet Configuration item based on the lookup key within the BACnetCommandRequestModel that it recieves.
        ///     In addition it will store the entire BACnet configure in a singleton object "IBACnetConfigMgr bACnetConfigMgr"
        ///     TODO: Currently this singleton object is retrieved on each call. But the goal is to use this as a cache, what is preventing this is the need for a change of value event comming from the System API.
        ///     This would keep the cache in sync with the database.
        /// </summary>
        /// <param name="bACnetCommandRequestModel"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="clientAPITokenMgr"></param>
        /// <param name="hmacSettings"></param>
        /// <param name="apiCommSettings"></param>
        /// <param name="bACnetConfigMgr"></param>
        /// <returns></returns>
        public async Task<BACnetCommandResponseModel> GetSystemValue(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, 
            IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            try
            {
                // TODO - currently this gets repeated for each property even though the System API returns all the properties. Not good.
                // Set up an IOptions<dataobject> that contains each of these.
                // So first check for a blank value, if blank make the call below and put these values into a IOptions<dataobject>,
                // On susequent calls here, if non-blank, just get the values from the IOptions<dataobject>

                BACnetCommandResponseModel bACnetCommandResponseModel = await Task.Run(() => GetSystemValueFromSystemApi(bACnetCommandRequestModel, httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings, bACnetConfigMgr));
                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        private static async Task<BACnetCommandResponseModel> GetSystemValueFromSystemApi(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, 
            IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            try
            {
                BACnetSystemConfigResponseModel bACnetSystemConfigResponseModel;
                BACnetCommandResponseModel bACnetCommandResponseModel = new BACnetCommandResponseModel();

                // For now make sure the configuration is up to date. In the future this singleton object should be updated on change.
                bACnetSystemConfigResponseModel = await bACnetConfigMgr.GetBACnetConfig(httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings);
               
                Result result = new Result();
                result.Disposition = "0";

                switch (bACnetCommandRequestModel.Params.LookupKey)
                {
                    case BACnetConstants.BACnetConfigEnabled:         // BACnet Enabled
                        if (bACnetSystemConfigResponseModel.BacNetEnabled.Equals(true))
                            result.Value = "1";
                        else
                            result.Value = "0";
                        break;
                    case BACnetConstants.BACnetConfigAreaControllerInstanceId:  // RouterDeviceId
                        // This needs to be changed to an int or string representation of an integer in the Config tables
                        result.Value = bACnetSystemConfigResponseModel.RouterInstanceId.ToString();
                        break;
                    case BACnetConstants.BACnetConfigAreaControllerInstanceName:   // RouterName
                        result.Value = bACnetSystemConfigResponseModel.RouterInstanceName;
                        break;
                    case BACnetConstants.BACnetConfigVirtualNetworkNumber:                    //VirtualNetworkNumber
                        result.Value = bACnetSystemConfigResponseModel.Vnet.ToString();
                        break;
                    case BACnetConstants.BACnetConfigPort:
                        result.Value = bACnetSystemConfigResponseModel.Port.ToString();
                        break;
                    case BACnetConstants.BACnetConfigBBMDEnabled:     //ForeignDeviceRegistrationEnabled
                        if (bACnetSystemConfigResponseModel.BbmdEnabled.Equals(true))
                            result.Value = "1";
                        else
                            result.Value = "0";
                        break;
                    case BACnetConstants.BACnetConfigBBMDIp:          //BBMDIpAddress
                        result.Value = bACnetSystemConfigResponseModel.BbmdIpAddress;
                        break;
                    case BACnetConstants.BACnetConfigBBMDTTL:  //
                        // This needs to be changed to an int or string representation of an integer in the Config tables
                        result.Value = bACnetSystemConfigResponseModel.BbmdTimeToLive.ToString();
                        break;
                    case BACnetConstants.BACnetConfigGatewayInstanceId:
                        result.Value = bACnetSystemConfigResponseModel.GatewayInstanceId.ToString();
                        break;
                    case BACnetConstants.BACnetConfigPhysicalNetworkNumber:
                        result.Value = bACnetSystemConfigResponseModel.PhysicalNetworkNumber.ToString();
                        break;
                    case BACnetConstants.BACnetConfigPhysicalNetworkNumberStatus:
                        result.Value = bACnetSystemConfigResponseModel.PhysicalNetworkNumberStatus.ToString();
                        break;

                    default:
                        result.Value = BACnetConstants.BACnetConfigNoMatch;
                        result.Disposition = "1";
                        break;

                }

                // IN THE FUTURE (once we upgrade BACnet and recertify we can make larger changes to nx-bacnet, we may want to get the entire configuration at once.
                // or selected params. For that we need to change BACnetCommandRequestModel to make BACnetCommandParamsModel a List inside the master model. Then use code like this to foreach through it.
                // Perhaps add an "all" case.
                //List<BACnetCommandParamsModel> bACnetCommandParamsModelList = bACnetCommandRequestModel.Params;
                //foreach (BACnetCommandParamsModel param in bACnetCommandParamsModelList)
                //{
                // The BACnetCommandResponseModel will need to be changed to include a list of name-value pairs and should be populated here
                // Based on the values in the request model
                //}

                bACnetCommandResponseModel.Result = result;
                bACnetCommandResponseModel.Id = 8;

                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }


        /// <summary>
        /// SetSystemValue - This API will receive a request from the BACnet Adapter (nx-bacnet) to update either the "physical-network-number" or "physical-network-number-status"
        ///     This API will update those values in the database.
        /// </summary>
        /// <param name="bACnetCommandRequestModel"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="clientAPITokenMgr"></param>
        /// <param name="hmacSettings"></param>
        /// <param name="apiCommSettings"></param>
        /// <param name="bACnetConfigMgr"></param>
        /// <returns></returns>
        public async Task<BACnetCommandResponseModel> SetSystemValue(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr,
            IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            try
            {
                BACnetCommandResponseModel bACnetCommandResponseModel = await Task.Run(() =>
                    SetSystemValueThruSystemApi(bACnetCommandRequestModel, httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings, bACnetConfigMgr));
                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        private async Task<BACnetCommandResponseModel> SetSystemValueThruSystemApi(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr,
            IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            BACnetCommandResponseModel bACnetCommandResponseModel = new BACnetCommandResponseModel();
            bACnetCommandResponseModel.Result = new Result();
            ResponseModel responseModel;
            try
            {
                if (bACnetCommandRequestModel.Params.Name == BACnetConstants.BACnetConfigPhysicalNetworkNumber || bACnetCommandRequestModel.Params.Name == BACnetConstants.BACnetConfigPhysicalNetworkNumberStatus)
                {
                    UpdateBACnetPhysicalNetworkPropsModel updateBACnetPhysicalNetworkPropsModel = new();

                    if (bACnetCommandRequestModel.Params.Name == BACnetConstants.BACnetConfigPhysicalNetworkNumber)
                    {
                        updateBACnetPhysicalNetworkPropsModel.UpdateStatus = false;
                        updateBACnetPhysicalNetworkPropsModel.PhysicalNetworkNumber = bACnetCommandRequestModel.Params.Set.Value.ToInt();
                    }
                    else if (bACnetCommandRequestModel.Params.Name == BACnetConstants.BACnetConfigPhysicalNetworkNumberStatus)
                    {
                        updateBACnetPhysicalNetworkPropsModel.UpdateStatus = true;
                        updateBACnetPhysicalNetworkPropsModel.PhysicalNetworkNumberStatus = bACnetCommandRequestModel.Params.Set.Value.ToInt();
                    }
                    responseModel = await bACnetConfigMgr.UpdateBACnetPhysicalNetworkProps(httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings, updateBACnetPhysicalNetworkPropsModel);
                    bACnetCommandResponseModel.Result.Value = bACnetCommandRequestModel.Params.Value;
                    bACnetCommandResponseModel.Result.Disposition = "0";
                    bACnetCommandResponseModel.Id = 8;
                }
                else
                {
                    bACnetCommandResponseModel.Result.Value = "0";
                    bACnetCommandResponseModel.Result.Disposition = "1";
                }

                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }


        /// <summary>
        /// GetZoneList - Gets a list of Zones for a particular NX Adapter
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="clientAPITokenMgr"></param>
        /// <param name="hmacSettings"></param>
        /// <param name="apiCommSettings"></param>
        /// <returns></returns>
        public async Task<BACnetZoneListResponseModel> GetZoneList(IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings)
        {
            try
            {
                BACnetZoneListResponseModel bACnetZoneListResponseModel = await Task.Run(() => GetZoneListFromSpaceApi(httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings));
                //BACnetCommandResponseModel bACnetCommandResponseModel = await Task.Run(() => new BACnetCommandResponseModel());
                return bACnetZoneListResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        private static async Task<BACnetZoneListResponseModel> GetZoneListFromSpaceApi(IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings)
        {
            try
            {
                BACnetCommandResponseModel bACnetCommandResponseModel = new BACnetCommandResponseModel();

                var httpClient = httpClientFactory.CreateClient();

                // Reusing the HMAC Settings model, but assuming a list of one since this is the client so only 1 set of settings.
                string accessToken = await clientAPITokenMgr.GetAccessToken(hmacSettings.Value.KeySettings[0].AppId, hmacSettings.Value.KeySettings[0].Key, 10, httpClient, apiCommSettings);

                string SpaceAPIUrl = apiCommSettings.Value.SpaceAPIUrl + Constants.SpaceApiZoneRead;

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, SpaceAPIUrl);

                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);

                BACnetZoneListResponseModel bACnetZoneListResponseModel = new BACnetZoneListResponseModel();
                List<BACnetZonelistParamsModel> bACnetZonelistParamsModels = new List<BACnetZonelistParamsModel>();
                Result BACnetCommandResponseModel = new Result();

                Logger.Debug(httpResponse.Content.ReadAsStringAsync().Result);

                if (httpResponse.IsSuccessStatusCode)
                {
                    List<ZoneListConfigResponseModel> zoneListConfigResponseModels = await httpResponse.Content.ReadFromJsonAsync<List<ZoneListConfigResponseModel>>();

                    foreach (ZoneListConfigResponseModel zoneListConfigResponseModel in zoneListConfigResponseModels)
                    {
                        var bACnetZonelistParamsModel = new BACnetZonelistParamsModel();
                        bACnetZonelistParamsModel.Id = zoneListConfigResponseModel.ExtZoneId;
                        bACnetZonelistParamsModel.Name = zoneListConfigResponseModel.Name;
                        bACnetZonelistParamsModel.Description = zoneListConfigResponseModel.Description;
                        bACnetZonelistParamsModels.Add(bACnetZonelistParamsModel);
                    }
                }
                bACnetZoneListResponseModel.Result = bACnetZonelistParamsModels;
                return bACnetZoneListResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }


        /// <summary>
        /// GetZoneProperties - Gets a list of Devices, Groups, and Presets in the form of the BACnetZoneProperties model for a given Zone. 
        /// </summary>
        /// <param name="bACnetCommandRequestModel"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="clientAPITokenMgr"></param>
        /// <param name="hmacSettings"></param>
        /// <param name="apiCommSettings"></param>
        /// <param name="bACnetConfigMgr"></param>
        /// <returns></returns>
        public async Task<BACnetZonePropertiesResponseModel> GetZoneProperties(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            try
            {
                BACnetZonePropertiesResponseModel bACnetZonePropertiesResponseModel = await Task.Run(() => GetZonePropertiesFromDeviceAPI(bACnetCommandRequestModel, httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings, bACnetConfigMgr));
                return bACnetZonePropertiesResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        private static async Task<BACnetZonePropertiesResponseModel> GetZonePropertiesFromDeviceAPI(BACnetCommandRequestModel bACnetCommandRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            try
            {
                List<ZoneModel> zoneModels;
                BACnetZonePropertiesResponseModel bACnetZonePropertiesResponseModel = new BACnetZonePropertiesResponseModel();

                BACnetSystemConfigResponseModel bACnetSystemConfigResponseModel = await bACnetConfigMgr.GetBACnetConfig(httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings);

                var httpClient = httpClientFactory.CreateClient();

                // Reusing the HMAC Settings model, but assuming a list of one since this is the client so only 1 set of settings.
                string accessToken = await clientAPITokenMgr.GetAccessToken(hmacSettings.Value.KeySettings[0].AppId, hmacSettings.Value.KeySettings[0].Key, 10, httpClient, apiCommSettings);

                string deviceAPIurl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiZoneDetailsRead;

                FilterModel requestContent = new FilterModel();
                requestContent.ZoneExternalId = bACnetCommandRequestModel.Params.Id;
                requestContent.NxAdapterId = bACnetSystemConfigResponseModel.NXAdapterId.ToGuid();

                var jsonContent = JsonContent.Create(requestContent);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, deviceAPIurl);

                httpRequest.Content = jsonContent;

                Logger.Debug(httpRequest.Content.ReadAsStringAsync().Result);

                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);

                Logger.Debug(httpResponse.Content.ReadAsStringAsync().Result);

                if (httpResponse.IsSuccessStatusCode)
                {
                    bACnetZonePropertiesResponseModel.Result = new ZonePropertiesResult();

                    zoneModels = await httpResponse.Content.ReadFromJsonAsync<List<ZoneModel>>();

                    //TODO if there is more than one zone model, need to error. External Zone Id should be unique and return only one. 

                    foreach (ZoneModel zoneModel in zoneModels)
                    {
                        bACnetZonePropertiesResponseModel.Result.Name = zoneModel.Name;
                        if (zoneModel.Description.IsNullOrEmpty())
                            bACnetZonePropertiesResponseModel.Result.Location = "UnSpecified";
                        else
                            bACnetZonePropertiesResponseModel.Result.Location = zoneModel.Description;

                        if (zoneModel.Devices.Count >= 1)
                        {
                            foreach (DeviceDTO deviceDTO in zoneModel.Devices)
                            {
                                if ((deviceDTO.CategoryName == Constants.DeviceFunctionCatDimmer || deviceDTO.CategoryName == Constants.DeviceFunctionCatRelay) && deviceDTO.ExtInstanceNumber <= 99)
                                {
                                    // If it is a dimmer or relay create the AV90XX or BV80XX Object, for the individual Dimmer or Relay BACnet Object
                                    if (bACnetZonePropertiesResponseModel.Result.Devices == null)
                                        bACnetZonePropertiesResponseModel.Result.Devices = new List<DetailsResult>();

                                    // See if the current NX Device category, Dimmers or Relays is one of the selected Export Objects if so add the device.
                                    if ((deviceDTO.CategoryName == Constants.DeviceFunctionCatDimmer) && (bACnetSystemConfigResponseModel.ExportObjects.Any(exportObject => exportObject.Name == BACnetConstants.BACnetObjectDimmer && exportObject.Selected == true)) ||
                                        ((deviceDTO.CategoryName == Constants.DeviceFunctionCatRelay) && (bACnetSystemConfigResponseModel.ExportObjects.Any(exportObject => exportObject.Name == BACnetConstants.BACnetObjectRelay && exportObject.Selected == true))))
                                        bACnetZonePropertiesResponseModel.Result.Devices
                                            .Add(BACnetObjectMapper.CreateDimmerOrRelayObj(deviceDTO, zoneModel.AreaNumber, zoneModel.ZoneNumber));

                                    // Here we are looking to add to the Groups section of the output.
                                    foreach (GroupModel group in zoneModel.Groups)
                                    {
                                        // See if the current NX Device belongs to any groups and see if Groups is selected as an Export Object if so proceed
                                        if (group.Devices.Any(device => device.Id.ToString() == deviceDTO.Id) && bACnetSystemConfigResponseModel.ExportObjects.Any(exportObject => exportObject.Name == BACnetConstants.BACnetObjectGroups && exportObject.Selected == true))
                                        {
                                            // If the dimmer or relay is in a group, create the AV2XX or BV1XX Object, for the Group Dimmer or Relay BACnet Object
                                            if (bACnetZonePropertiesResponseModel.Result.Groups == null)
                                                bACnetZonePropertiesResponseModel.Result.Groups = new List<DetailsResult>();

                                            // Only 1 Relay and 1 Dimmer Per Group, Do not add more than one each
                                            string tmpId;
                                            if (deviceDTO.CategoryName == Constants.DeviceFunctionCatDimmer)
                                                tmpId = BACnetConstants.GroupDimmerObjectType.Substring(2,1) + group.GroupNumber.ToString("D2");     // Dimmer-Group Ids start with 2 
                                            else
                                                tmpId = BACnetConstants.GroupRelayObjectType.Substring(2,1) + group.GroupNumber.ToString("D2");     // Relay-Group Ids start with 1

                                            if (!bACnetZonePropertiesResponseModel.Result.Groups.Any(groupObj => groupObj.id == tmpId))
                                            {
                                                bACnetZonePropertiesResponseModel.Result.Groups
                                                    .Add(BACnetObjectMapper.CreateGroupDimmerOrRelayObj(deviceDTO, zoneModel.AreaNumber, zoneModel.ZoneNumber, group.GroupNumber));
                                            }
                                        }
                                    }
                                }
                                // See if the current NX Device is an Occ Sensor and if Occ Sensors are a selected Export Object
                                else if ((deviceDTO.CategoryName == Constants.DeviceFunctionCatOccSensor) && (bACnetSystemConfigResponseModel.ExportObjects.Any(exportObject => exportObject.Name == BACnetConstants.BACnetObjectOcc && exportObject.Selected == true)))
                                {
                                    if ((bACnetZonePropertiesResponseModel.Result.Devices == null) || (!bACnetZonePropertiesResponseModel.Result.Devices.Any(device => device.device_type == BACnetConstants.ZoneOccupancyObjectType)))
                                    {
                                        // If there NOT a BV301 Object already populated and we just found one create it, there should only be one Zone Occupancy BACnet Object
                                        if (bACnetZonePropertiesResponseModel.Result.Devices == null)
                                            bACnetZonePropertiesResponseModel.Result.Devices = new List<DetailsResult>();

                                        bACnetZonePropertiesResponseModel.Result.Devices
                                            .Add(BACnetObjectMapper.CreateZoneOccObj(deviceDTO, zoneModel.AreaNumber, zoneModel.ZoneNumber));
                                    }
                                }
                                // See if the current NX Device is Power Capable and Wattage is a selected Export Object
                                else if ((deviceDTO.PowerCapabilityCode == Constants.DevicePowerCapPowerCapable) && (bACnetSystemConfigResponseModel.ExportObjects.Any(exportObject => exportObject.Name == BACnetConstants.BACnetObjectPower && exportObject.Selected == true)))
                                {
                                    if ((bACnetZonePropertiesResponseModel.Result.Devices == null) || (!bACnetZonePropertiesResponseModel.Result.Devices.Any(device => device.device_type == BACnetConstants.ZonePowerObjectType)))
                                    {
                                        // If there NOT a AV701 Object already populated and we just found one create it, there should only be one Zone Power BACnet Object
                                        if (bACnetZonePropertiesResponseModel.Result.Devices == null)
                                            bACnetZonePropertiesResponseModel.Result.Devices = new List<DetailsResult>();

                                        bACnetZonePropertiesResponseModel.Result.Devices
                                            .Add(BACnetObjectMapper.CreateZonePowerObj(deviceDTO, zoneModel.AreaNumber, zoneModel.ZoneNumber));
                                    }
                                }
                                // See if the current NX Device is a Photocell and if Photocells are a selected Export Object
                                else if ((deviceDTO.CategoryName == Constants.DeviceFunctionCatPhotocell) && (bACnetSystemConfigResponseModel.ExportObjects.Any(exportObject => exportObject.Name == BACnetConstants.BACnetObjectPhotocell && exportObject.Selected == true)))
                                {
                                    foreach (GroupModel group in zoneModel.Groups)
                                    {
                                        if (group.Devices.Any(device => device.Id.ToString() == deviceDTO.Id))
                                        {
                                            // If the photocell in a group, create the AV6XX Object, for the Photocell BACnet Object
                                            if (bACnetZonePropertiesResponseModel.Result.Devices == null)
                                                bACnetZonePropertiesResponseModel.Result.Devices = new List<DetailsResult>();

                                            bACnetZonePropertiesResponseModel.Result.Devices
                                                .Add(BACnetObjectMapper.CreateGroupPhotocellObj(deviceDTO, zoneModel.AreaNumber, zoneModel.ZoneNumber, group.GroupNumber));
                                        }
                                    }
                                }
                            }
                            // If Presets are a selected Export Object type process any presets for output
                            if (bACnetSystemConfigResponseModel.ExportObjects.Any(exportObject => exportObject.Name == BACnetConstants.BACnetObjectPresets && exportObject.Selected == true))
                            {
                                foreach (PresetModel preset in zoneModel.Presets)
                                {
                                    if (bACnetZonePropertiesResponseModel.Result.Presets == null)
                                        bACnetZonePropertiesResponseModel.Result.Presets = new List<DetailsResult>();

                                    bACnetZonePropertiesResponseModel.Result.Presets.Add(BACnetObjectMapper.CreatePresetObj(preset, zoneModel.AreaNumber, zoneModel.ZoneNumber));
                                }
                            }
                        }
                    }
                }
                return bACnetZonePropertiesResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        /// <summary>
        /// GetNxValue - Retrieves the state or present value of a specific NX Device, or grouping of devices that form a specific BACnet Object. 
        /// </summary>
        /// <param name="nxValueRequestModel"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="clientAPITokenMgr"></param>
        /// <param name="hmacSettings"></param>
        /// <param name="apiCommSettings"></param>
        /// <returns></returns>
        public async Task<BACnetCommandResponseModel> GetNxValue(NxValueRequestModel nxValueRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            try
            {
                BACnetCommandResponseModel bACnetCommandResponseModel = await Task.Run(() => GetNxValueFromDeviceApi(nxValueRequestModel, httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings, bACnetConfigMgr));
                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }
        private async Task<BACnetCommandResponseModel> GetNxValueFromDeviceApi(NxValueRequestModel nxValueRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            BACnetCommandResponseModel bACnetCommandResponseModel = new BACnetCommandResponseModel();
            bACnetCommandResponseModel.Result = new Result();
            try
            {
                BACnetSystemConfigResponseModel bACnetSystemConfigResponseModel;
                JsonContent jsonContent = null;
                FilterModel filterModel = new();
                AdapterExchange adapterExchange = new();
                BACnetGetNxValueResponseModel bACnetGetNxValueResponseModel = new BACnetGetNxValueResponseModel();
                var httpClient = httpClientFactory.CreateClient();
                string deviceApiUrl = null;
                List<DeviceDetailModel> deviceDetailModels;
                DeviceDetailModel deviceDetailModel;
                string bacnetObjectCode;

                bACnetSystemConfigResponseModel = await bACnetConfigMgr.GetBACnetConfig(httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings);

                filterModel.NxAdapterId = bACnetSystemConfigResponseModel.NXAdapterId.ToGuid();

                string accessToken = await clientAPITokenMgr.GetAccessToken(hmacSettings.Value.KeySettings[0].AppId, hmacSettings.Value.KeySettings[0].Key, 10, httpClient, apiCommSettings);
                string[] bacnetParamNameString = nxValueRequestModel.Params.Name.Split('-'); //splits the name 2-1-D-1301-BV8001 separated by a "-"

                if ((bacnetParamNameString[4].Substring(0, 4) == BACnetConstants.DeviceRelayObjectType) ||
                    (bacnetParamNameString[4].Substring(0, 4) == BACnetConstants.DeviceDimmerObjectType))
                    bacnetObjectCode = bacnetParamNameString[4].Substring(0, 4);
                else
                    bacnetObjectCode = bacnetParamNameString[4].Substring(0, 3);

                switch (bacnetObjectCode)
                {
                    case BACnetConstants.ZonePowerObjectType: //Zone Watts
                        deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiZonePowerRead;
                        filterModel.AreaNumber = Convert.ToInt32(bacnetParamNameString[0]);
                        filterModel.ZoneNumber = Convert.ToInt32(bacnetParamNameString[1]);
                        jsonContent = JsonContent.Create(filterModel);
                        break;
                    case BACnetConstants.ZoneOccupancyObjectType: //Occupancy state
                        deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiZoneOccupancyRead;
                        filterModel.AreaNumber = Convert.ToInt32(bacnetParamNameString[0]);
                        filterModel.ZoneNumber = Convert.ToInt32(bacnetParamNameString[1]);
                        jsonContent = JsonContent.Create(filterModel);
                        break;
                    case BACnetConstants.GroupRelayObjectType:
                        deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiGroupRelayStateRead;
                        filterModel.AreaNumber = Convert.ToInt32(bacnetParamNameString[0]);
                        filterModel.ZoneNumber = Convert.ToInt32(bacnetParamNameString[1]);
                        filterModel.GroupNumber = Convert.ToInt32(bacnetParamNameString[3]);
                        jsonContent = JsonContent.Create(filterModel);
                        break;
                    case BACnetConstants.GroupDimmerObjectType:
                        deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiGroupDimmerLevelRead;
                        filterModel.AreaNumber = Convert.ToInt32(bacnetParamNameString[0]);
                        filterModel.ZoneNumber = Convert.ToInt32(bacnetParamNameString[1]);
                        filterModel.GroupNumber = Convert.ToInt32(bacnetParamNameString[3]);
                        jsonContent = JsonContent.Create(filterModel);
                        break;
                    case BACnetConstants.GroupLightLevelObjectType:                                                        
                        deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiGroupDaylightRead;
                        filterModel.AreaNumber = Convert.ToInt32(bacnetParamNameString[0]);
                        filterModel.ZoneNumber = Convert.ToInt32(bacnetParamNameString[1]);
                        filterModel.GroupNumber = Convert.ToInt32(bacnetParamNameString[4].Substring(3, 2).ToInt().ToString());
                        jsonContent = JsonContent.Create(filterModel);
                        break;

                    case BACnetConstants.DeviceRelayObjectType:
                    case BACnetConstants.DeviceDimmerObjectType:
                        deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiDeviceRelayStateRead;
                        adapterExchange.Header = BACnetConstants.AdapterGetMessageHeader;
                        AdapterDevice adapterDevice = new AdapterDevice
                        {
                            Common = new Dictionary<string, string>
                            {
                                {Constants.AdapterExchangeCommonKeyExternalId, bacnetParamNameString[3] }
                            },
                            Properties = new Dictionary<string, string>
                            {
                                {"",""}
                            }
                        };
                        List<AdapterDevice> adapterDevices = new List<AdapterDevice>();
                        adapterDevices.Add(adapterDevice);
                        adapterExchange.Devices = adapterDevices.ToArray();
                        jsonContent = JsonContent.Create(adapterExchange);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException("BACnet Object Type", "No Match");
                }

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, deviceApiUrl);

                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpRequestMessage.Content = jsonContent;

                Logger.Debug(httpRequestMessage.Content.ReadAsStringAsync().Result);

                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequestMessage);

                Logger.Debug(httpResponse.Content.ReadAsStringAsync().Result);

                if (httpResponse.IsSuccessStatusCode)
                {
                    switch (bacnetObjectCode)
                    {
                        case BACnetConstants.ZonePowerObjectType: //Zone Watts
                        case BACnetConstants.ZoneOccupancyObjectType: //Occupancy state
                        case BACnetConstants.GroupRelayObjectType:
                        case BACnetConstants.GroupDimmerObjectType:
                        case BACnetConstants.GroupLightLevelObjectType:
                            bACnetCommandResponseModel.Result.present_value = await httpResponse.Content.ReadAsStringAsync();
                            break;
                        case BACnetConstants.DeviceRelayObjectType:
                            deviceDetailModels = await httpResponse.Content.ReadFromJsonAsync<List<DeviceDetailModel>>();
                            deviceDetailModel = deviceDetailModels.FirstOrDefault();
                            bACnetCommandResponseModel.Result.present_value 
                                = deviceDetailModel.Properties.Single(prop => prop.Key == PropertyConstant.RelayState).Value;
                            break;
                        case BACnetConstants.DeviceDimmerObjectType:
                            deviceDetailModels = await httpResponse.Content.ReadFromJsonAsync<List<DeviceDetailModel>>();
                            deviceDetailModel = deviceDetailModels.FirstOrDefault();
                            bACnetCommandResponseModel.Result.present_value
                                = deviceDetailModel.Properties.Single(prop => prop.Key == PropertyConstant.DimmerLevel).Value;
                            break;
                    }
                    bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
                    bACnetCommandResponseModel.Result.Value = null;
                    bACnetCommandResponseModel.Result.Disposition = BACnetConstants.ResponseDispositionSuccess;
                    bACnetCommandResponseModel.Id = 6;
                }
                else
                {
                    bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
                    bACnetCommandResponseModel.Result.Value = null;
                    bACnetCommandResponseModel.Result.present_value = null;
                    bACnetCommandResponseModel.Result.Disposition = BACnetConstants.ResponseDispositionFailure;
                    bACnetCommandResponseModel.Id = 6;
                }
                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                //throw new EHubbException(ex);
                bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
                bACnetCommandResponseModel.Result.Value = null;
                bACnetCommandResponseModel.Result.present_value = null;
                bACnetCommandResponseModel.Result.Disposition = BACnetConstants.ResponseDispositionFailure;
                bACnetCommandResponseModel.Id = 6;

                return bACnetCommandResponseModel;
            }
        }

        /// <summary>
        /// SetNxValue - Sets the state of a specific NX Devices or grouping of devices based on the value given and the grouping defined by the BACnet Object specified.
        /// </summary>
        /// <param name="nxValueRequestModel"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="clientAPITokenMgr"></param>
        /// <param name="hmacSettings"></param>
        /// <param name="apiCommSettings"></param>
        /// <param name="bACnetConfigMgr"></param>
        /// <returns></returns>
        public async Task<BACnetCommandResponseModel> SetNxValue(NxValueRequestModel nxValueRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, 
            IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            try
            {
                BACnetCommandResponseModel bACnetCommandResponseModel = await Task.Run(() => 
                    SetNxValueThruDeviceApi(nxValueRequestModel, httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings, bACnetConfigMgr));
                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        private async Task<BACnetCommandResponseModel> SetNxValueThruDeviceApi(NxValueRequestModel nxValueRequestModel, IHttpClientFactory httpClientFactory, IClientAPITokenMgr clientAPITokenMgr, 
            IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, IBACnetConfigMgr bACnetConfigMgr)
        {
            BACnetCommandResponseModel bACnetCommandResponseModel = new BACnetCommandResponseModel();
            bACnetCommandResponseModel.Result = new Result();
            BACnetSystemConfigResponseModel bACnetSystemConfigResponseModel;
            try
            {
                JsonContent jsonContent = null;
                FilterModel filterModel = new();
                BACnetGetNxValueResponseModel bACnetGetNxValueResponseModel = new BACnetGetNxValueResponseModel();
                var httpClient = httpClientFactory.CreateClient();
                string deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiAsynchronousRoute;
                AdapterExchange adapterExchange = new();
                AdapterDevice adapterDevice;
                List<AdapterDevice> adapterDevices;
                adapterExchange.Header = BACnetConstants.AdapterSetMessageHeader;
                
                // For now make sure the configuration is up to date. In the future this singleton object should be updated on change.
                bACnetSystemConfigResponseModel = await bACnetConfigMgr.GetBACnetConfig(httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings);

                string accessToken = await clientAPITokenMgr.GetAccessToken(hmacSettings.Value.KeySettings[0].AppId, hmacSettings.Value.KeySettings[0].Key, 10, httpClient, apiCommSettings);
                string[] bacnetParamNameString = nxValueRequestModel.Params.Name.Split('-'); //splits the name 2-1-D-1301-BV8001 separated by a "-"
                string[] zoneNumbers = {""};
                string[] groupNumbers = {""};
                zoneNumbers[0] = bacnetParamNameString[1];
                string areaNumber = bacnetParamNameString[0];
                string bacnetObjectCode;
                string groupOrExtDeviceId = bacnetParamNameString[3];

                if ((bacnetParamNameString[4].Substring(0, 4) == BACnetConstants.DeviceRelayObjectType) ||
                    (bacnetParamNameString[4].Substring(0, 4) == BACnetConstants.DeviceDimmerObjectType))
                    bacnetObjectCode = bacnetParamNameString[4].Substring(0, 4);
                else
                    bacnetObjectCode = bacnetParamNameString[4].Substring(0, 3);

                switch (bacnetObjectCode) //choosing device type based on number. For instance, BV8 belongs to a relay
                {
                    case BACnetConstants.GroupRelayObjectType:
                    case BACnetConstants.GroupDimmerObjectType:

                        adapterExchange.Multicast = new AdapterMultiCast();
                        adapterExchange.Multicast.Destination = new MulticastDestination();
                        adapterExchange.Multicast.Destination.NxAdapterId = bACnetConfigMgr.GetBACnetConfig().NXAdapterId;
                        adapterExchange.Multicast.Destination.AreaNumber = areaNumber;

                        adapterExchange.Multicast.Destination.ZoneNumbers = zoneNumbers;
                        groupNumbers[0] = groupOrExtDeviceId;
                        adapterExchange.Multicast.Destination.Groups = groupNumbers;

                        adapterExchange.Multicast.Settings = new MultiCastSettings();
                        adapterExchange.Multicast.Settings.Properties = new Dictionary<string, string>();

                        if (bacnetObjectCode == BACnetConstants.GroupRelayObjectType)
                            adapterExchange.Multicast.Settings.Properties.Add(PropertyConstant.RelayState, nxValueRequestModel.Params.Set.present_value);
                        else
                            adapterExchange.Multicast.Settings.Properties.Add(PropertyConstant.DimmerLevel, nxValueRequestModel.Params.Set.present_value);

                        jsonContent = JsonContent.Create(adapterExchange);

                        break;
                    case BACnetConstants.DeviceRelayObjectType:
                        
                        adapterDevice = new AdapterDevice
                        {
                            Common = new Dictionary<string, string>
                            {
                                {Constants.AdapterExchangeCommonKeyExternalId, groupOrExtDeviceId }
                            },
                            Properties = new Dictionary<string, string>
                            {
                                {Constants.AdapterDevicePropertyRelayState, nxValueRequestModel.Params.Set.present_value}
                            }
                        };
                        adapterDevices = new List<AdapterDevice>();
                        adapterDevices.Add(adapterDevice);
                        adapterExchange.Devices = adapterDevices.ToArray();
                        jsonContent = JsonContent.Create(adapterExchange);

                        break;
                    case BACnetConstants.DeviceDimmerObjectType:
 
                        adapterDevice = new AdapterDevice
                        {
                            Common = new Dictionary<string, string>
                            {
                                {Constants.AdapterExchangeCommonKeyExternalId, groupOrExtDeviceId }
                            },
                            Properties = new Dictionary<string, string>
                            {
                                {Constants.AdapterDevicePropertyDimmerLevel, nxValueRequestModel.Params.Set.present_value}
                            }
                        };
                        adapterDevices = new List<AdapterDevice>();
                        adapterDevices.Add(adapterDevice);
                        adapterExchange.Devices = adapterDevices.ToArray();
                        jsonContent = JsonContent.Create(adapterExchange);

                        break;
                    case BACnetConstants.PresetObjectType:

                        PresetFilterModel presetFilterModel = new PresetFilterModel();
                        presetFilterModel.NxAdapterId = bACnetConfigMgr.GetBACnetConfig().NXAdapterId.ToGuid();
                        presetFilterModel.AreaNumber = areaNumber.ToInt();
                        presetFilterModel.ZoneNumber = zoneNumbers[0].ToInt();
                        presetFilterModel.PresetNumber = nxValueRequestModel.Params.Set.present_value.ToInt();


                        jsonContent = JsonContent.Create(presetFilterModel);
                        deviceApiUrl = apiCommSettings.Value.DeviceAPIUrl + Constants.DeviceApiPresetActivate;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException("BACnet Object Type", "No Match");
                }

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, deviceApiUrl);

                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpRequestMessage.Content = jsonContent;

                Logger.Debug(httpRequestMessage.Content.ReadAsStringAsync().Result);

                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequestMessage);

                Logger.Debug(httpResponse.Content.ReadAsStringAsync().Result);

                if (httpResponse.IsSuccessStatusCode)
                {
                    bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
                    bACnetCommandResponseModel.Result.Value = null;
                    bACnetCommandResponseModel.Result.Disposition = BACnetConstants.ResponseDispositionSuccess;
                    bACnetCommandResponseModel.Id = 6;
                }
                else
                {
                    bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
                    bACnetCommandResponseModel.Result.Value = null;
                    bACnetCommandResponseModel.Result.present_value = null;
                    bACnetCommandResponseModel.Result.Disposition = BACnetConstants.ResponseDispositionFailure;
                    bACnetCommandResponseModel.Id = 6;
                }
                return bACnetCommandResponseModel;
            }
            catch (Exception ex)
            {
                //throw new EHubbException(ex);
                bACnetCommandResponseModel.Jsonrpc = BACnetConstants.BACnetRPCVersion;
                bACnetCommandResponseModel.Result.Value = null;
                bACnetCommandResponseModel.Result.present_value = null;
                bACnetCommandResponseModel.Result.Disposition = BACnetConstants.ResponseDispositionFailure;
                bACnetCommandResponseModel.Id = 6;

                return bACnetCommandResponseModel;
            }
        }
    }
}
