using Hubbell.EHubb.BACnetAPI.BusinessManager.Interfaces;
using Hubbell.EHubb.BACnetAPI.Models;
using Hubbell.EHubb.Common.APIComm.DataObjects;
using Hubbell.EHubb.Common.AppConstant;
using Hubbell.EHubb.Common.CommonModels;
using Hubbell.EHubb.Common.Exceptions;
using Hubbell.EHubb.Common.Extenstion;
using Hubbell.EHubb.Common.ResponseHandler;
using Hubbell.EHubb.Common.Security.Utility.Interface;
using Hubbell.EHubb.Common.TokenHandler.DataObject;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.BusinessManager
{
    public class BACnetConfigMgr : IBACnetConfigMgr
    {
        public BACnetSystemConfigResponseModel GetBACnetConfig()
        {
            return BACnetSystemConfig;
        }

        public async Task<BACnetSystemConfigResponseModel> GetBACnetConfig(IHttpClientFactory httpClientFactory,
            IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings)
        {
            BACnetSystemConfigResponseModel bACnetSystemConfigResponseModel;

            // TODO This singleton class was added to cache the data, but it also needs a way to update when changes are made.
            // Current thought is to add an Update Timestamp. LastUpdated in the Database also maintain lastUpdated in the object.
            // If the database was updated more recently re-get the data.
            // Will try this in the next WI. For now punt and get the data each time. So commenting out the if temporaraly.

            try
            {
                //if (BACnetSystemConfig == null)
                //{
                var httpClient = httpClientFactory.CreateClient();

                // Reusing the HMAC Settings model, but assuming a list of one since this is the client so only 1 set of settings.
                string accessToken = await clientAPITokenMgr.GetAccessToken(hmacSettings.Value.KeySettings[0].AppId, hmacSettings.Value.KeySettings[0].Key, 10, httpClient, apiCommSettings);

                string systemAPIurl = apiCommSettings.Value.SystemAPIUrl + Constants.SystemApiBACnetConfigRead;

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, systemAPIurl);

                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);

                if (httpResponse.IsSuccessStatusCode)
                {
                    bACnetSystemConfigResponseModel = await httpResponse.Content.ReadFromJsonAsync<BACnetSystemConfigResponseModel>();
                    SetBACnetConfig(bACnetSystemConfigResponseModel);
                }
                //}
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
            return BACnetSystemConfig;
        }

        public async Task<ResponseModel> UpdateBACnetPhysicalNetworkProps(IHttpClientFactory httpClientFactory,
            IClientAPITokenMgr clientAPITokenMgr, IOptions<HmacSetting> hmacSettings, IOptions<APICommDO> apiCommSettings, UpdateBACnetPhysicalNetworkPropsModel updateBACnetPhysicalNetworkPropsModel)
        {
            try
            {
                ResponseModel responseModel = new ResponseModel();
                var httpClient = httpClientFactory.CreateClient();

                // Reusing the HMAC Settings model, but assuming a list of one since this is the client so only 1 set of settings.
                string accessToken = await clientAPITokenMgr.GetAccessToken(hmacSettings.Value.KeySettings[0].AppId, hmacSettings.Value.KeySettings[0].Key, 10, httpClient, apiCommSettings);

                string systemAPIurl = apiCommSettings.Value.SystemAPIUrl + Constants.SystemApiBACnetUpdatePhysicalNetworkProps;

                // For now make sure the configuration is up to date. In the future this singleton object should be updated on change.
                BACnetSystemConfigResponseModel bACnetSystemConfigResponseModel = await GetBACnetConfig(httpClientFactory, clientAPITokenMgr, hmacSettings, apiCommSettings);

                updateBACnetPhysicalNetworkPropsModel.NxAdapterId = BACnetSystemConfig.NXAdapterId.ToGuid();

                var jsonContent = JsonContent.Create(updateBACnetPhysicalNetworkPropsModel);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, systemAPIurl);

                httpRequest.Content = jsonContent;

                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);

                if (httpResponse.IsSuccessStatusCode)
                {
                    responseModel = await httpResponse.Content.ReadFromJsonAsync<ResponseModel>();
                }
                return responseModel;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        public void SetBACnetConfig(BACnetSystemConfigResponseModel bNConfigModel)
        {
            BACnetSystemConfig = bNConfigModel;
        }

        private BACnetSystemConfigResponseModel BACnetSystemConfig { get; set; }
    }
}
