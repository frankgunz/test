using Hubbell.EHubb.BACnetAPI.Models;
using Hubbell.EHubb.Common.AppConstant;
using Hubbell.EHubb.Common.Exceptions;
using Hubbell.EHubb.Device.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.BusinessManager.Mapper
{
    public class BACnetObjectMapper
    {
        /// <summary>
        /// Create the AV90XX or BV80XX node for the BACnet Response 
        /// </summary>
        /// <param name="deviceDTO"></param>
        /// <returns></returns>
        public static DetailsResult CreateDimmerOrRelayObj(DeviceDTO deviceDTO, int areaNumber, int zoneNumber)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                DetailsResult detailsResult = new DetailsResult();

                string bNObjectTypeCode = "";
                string bNObjectIdCode = "";
                string extInstanceNumber;
                if (deviceDTO.ExtInstanceNumber != null)
                    extInstanceNumber = ((int)deviceDTO.ExtInstanceNumber).ToString("D2");
                else
                    extInstanceNumber = "00";

                if (deviceDTO.CategoryName == Constants.DeviceFunctionCatDimmer)
                {
                    bNObjectTypeCode = BACnetConstants.DeviceDimmerObjectType;
                    bNObjectIdCode = "90";
                }
                else if (deviceDTO.CategoryName == Constants.DeviceFunctionCatRelay)
                {
                    bNObjectTypeCode = BACnetConstants.DeviceRelayObjectType;
                    bNObjectIdCode = "80";
                }

                stringBuilder.Append(deviceDTO.Address)
                    .Append('-')
                    .Append(deviceDTO.DeviceTypeIdHex)
                    .Append(deviceDTO.DeviceInstance.ToString("D2"));
                detailsResult.description = stringBuilder.ToString();

                stringBuilder.Clear()
                    .Append(areaNumber)
                    .Append('-')
                    .Append(zoneNumber)
                    .Append('-')
                    .Append('D')
                    .Append('-')
                    .Append(deviceDTO.ExtDeviceId)
                    .Append('-')
                    .Append(bNObjectTypeCode)
                    .Append(extInstanceNumber);
                detailsResult.name = stringBuilder.ToString();
                detailsResult.export_name = deviceDTO.DeviceName;
                detailsResult.device_type = bNObjectTypeCode;

                stringBuilder.Clear()
                    .Append(bNObjectIdCode)
                    .Append(extInstanceNumber);
                detailsResult.id = stringBuilder.ToString();

                return detailsResult;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        public static DetailsResult CreateGroupDimmerOrRelayObj(DeviceDTO deviceDTO, int areaNumber, int zoneNumber, int groupNumber)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                DetailsResult detailsResult = new DetailsResult();
                string bNObjectTypeCode = "";
                string bNObjectIdCode = "";
                string descPostScript = "";

                if (deviceDTO.CategoryName == Constants.DeviceFunctionCatDimmer)
                {
                    bNObjectTypeCode = BACnetConstants.GroupDimmerObjectType;
                    bNObjectIdCode = "2";
                    descPostScript = "Dimmer";
                }
                else if (deviceDTO.CategoryName == Constants.DeviceFunctionCatRelay)
                {
                    bNObjectTypeCode = BACnetConstants.GroupRelayObjectType;
                    bNObjectIdCode = "1";
                    descPostScript = "Relay";
                }

                stringBuilder.Append("Group #")
                    .Append(groupNumber.ToString())
                    .Append('-')
                    .Append(descPostScript);
                detailsResult.description = stringBuilder.ToString();

                stringBuilder.Clear()
                    .Append(areaNumber)
                    .Append('-')
                    .Append(zoneNumber)
                    .Append('-')
                    .Append('G')
                    .Append('-')
                    .Append(groupNumber)
                    .Append('-')
                    .Append(bNObjectTypeCode)
                    .Append(groupNumber.ToString("D2"));
                detailsResult.name = stringBuilder.ToString();

                detailsResult.device_type = bNObjectTypeCode;

                stringBuilder.Clear()
                    .Append("Group ")
                    .Append(groupNumber.ToString())
                    .Append(' ')
                    .Append(descPostScript);
                detailsResult.export_name = stringBuilder.ToString();

                stringBuilder.Clear()
                    .Append(bNObjectIdCode)
                    .Append(groupNumber.ToString("D2"));
                detailsResult.id = stringBuilder.ToString();

                return detailsResult;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        public static DetailsResult CreateZoneOccObj(DeviceDTO deviceDTO, int areaNumber, int zoneNumber)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                DetailsResult detailsResult = new DetailsResult();

                stringBuilder.Append(deviceDTO.Address)
                   .Append('-')
                   .Append(deviceDTO.DeviceTypeIdHex)
                   .Append(deviceDTO.DeviceInstance.ToString("D2"));
                detailsResult.description = stringBuilder.ToString();

                stringBuilder.Clear()
                    .Append(areaNumber)
                    .Append('-')
                    .Append(zoneNumber)
                    .Append('-')
                    .Append('D')
                    .Append('-')
                    .Append(deviceDTO.ExtDeviceId)
                    .Append('-')
                    .Append("BV301");
                detailsResult.name = stringBuilder.ToString();
                detailsResult.export_name = deviceDTO.DeviceName;
                detailsResult.device_type = BACnetConstants.ZoneOccupancyObjectType;
                detailsResult.id = "301";

                return detailsResult;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        public static DetailsResult CreateZonePowerObj(DeviceDTO deviceDTO, int areaNumber, int zoneNumber)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                DetailsResult detailsResult = new DetailsResult();

                stringBuilder.Append(deviceDTO.Address)
                   .Append('-')
                   .Append(deviceDTO.DeviceTypeIdHex)
                   .Append(deviceDTO.DeviceInstance.ToString("D2"));
                detailsResult.description = stringBuilder.ToString();

                stringBuilder.Clear()
                    .Append(areaNumber)
                    .Append('-')
                    .Append(zoneNumber)
                    .Append('-')
                    .Append('D')
                    .Append('-')
                    .Append(deviceDTO.ExtDeviceId)
                    .Append('-')
                    .Append("AV701");
                detailsResult.name = stringBuilder.ToString();
                detailsResult.export_name = deviceDTO.DeviceName;
                detailsResult.device_type = BACnetConstants.ZonePowerObjectType;
                detailsResult.id = "701";

                return detailsResult;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        public static DetailsResult CreateGroupPhotocellObj(DeviceDTO deviceDTO, int areaNumber, int zoneNumber, int groupNumber)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                DetailsResult detailsResult = new DetailsResult();

                stringBuilder.Append(deviceDTO.Address)
                    .Append('-')
                    .Append(deviceDTO.DeviceTypeIdHex)
                    .Append(deviceDTO.DeviceInstance.ToString("D2"));
                detailsResult.description = stringBuilder.ToString();

                stringBuilder.Clear()
                    .Append(areaNumber)
                    .Append('-')
                    .Append(zoneNumber)
                    .Append('-')
                    .Append('D')
                    .Append('-')
                    .Append(deviceDTO.ExtDeviceId)
                    .Append('-')
                    .Append(BACnetConstants.GroupLightLevelObjectType)
                    .Append(groupNumber.ToString("D2"));
                detailsResult.name = stringBuilder.ToString();

                detailsResult.device_type = BACnetConstants.GroupLightLevelObjectType;
                detailsResult.export_name = deviceDTO.DeviceName;

                stringBuilder.Clear()
                    .Append('6')
                    .Append(groupNumber.ToString("D2"));
                detailsResult.id = stringBuilder.ToString();

                return detailsResult;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }

        public static DetailsResult CreatePresetObj(PresetModel preset, int areaNumber, int zoneNumber)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                DetailsResult detailsResult = new DetailsResult();

                stringBuilder.Append("Preset #")
                     .Append(preset.PresetNumber.ToString());
                detailsResult.description = stringBuilder.ToString();

                stringBuilder.Clear()
                    .Append(areaNumber)
                    .Append('-')
                    .Append(zoneNumber)
                    .Append('-')
                    .Append('P')
                    .Append('-')
                    .Append(preset.PresetNumber.ToString())
                    .Append('-')
                    .Append(BACnetConstants.PresetObjectType)
                    .Append(preset.PresetNumber.ToString("D2"));
                detailsResult.name = stringBuilder.ToString();

                detailsResult.device_type = BACnetConstants.PresetObjectType;

                detailsResult.export_name = preset.Name;

                stringBuilder.Clear()
                    .Append('4')
                    .Append(preset.PresetNumber.ToString("D2"));
                detailsResult.id = stringBuilder.ToString();

                return detailsResult;
            }
            catch (Exception ex)
            {
                throw new EHubbException(ex);
            }
        }
    }
}
