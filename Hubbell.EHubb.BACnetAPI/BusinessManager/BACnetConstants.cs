using Hubbell.EHubb.Device.Infrastructure.Helper;
using Hubbell.EHubb.Device.Infrastructure.Model.NxAdapterRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.BusinessManager
{
    public class BACnetConstants
    {
        public const string ZonePowerObjectType = "AV7";
        public const string ZoneOccupancyObjectType = "BV3";
        public const string GroupRelayObjectType = "BV1";
        public const string GroupDimmerObjectType = "AV2";
        public const string GroupLightLevelObjectType = "AV6";
        public const string DeviceRelayObjectType = "BV80";
        public const string DeviceDimmerObjectType = "AV90";
        public const string PresetObjectType = "MV4";

        public const string BACnetRPCVersion = "2.0";

        public const string ResponseDispositionSuccess = "0";
        public const string ResponseDispositionFailure = "1";

        public const string BACnetObjectOcc = "Occupancy";
        public const string BACnetObjectRelay = "Relays";
        public const string BACnetObjectDimmer = "Dimmers";
        public const string BACnetObjectPhotocell = "Photocells";
        public const string BACnetObjectPresets = "Presets";
        public const string BACnetObjectGroups = "Groups";
        public const string BACnetObjectPower = "Wattage";

        public const string BACnetConfigEnabled = "enabled";
        public const string BACnetConfigAreaControllerInstanceId = "area-controller-instance-id";
        public const string BACnetConfigAreaControllerInstanceName = "area-controller-instance-name";
        public const string BACnetConfigVirtualNetworkNumber = "vnet";
        public const string BACnetConfigPort = "bacnet-port";
        public const string BACnetConfigBBMDEnabled = "bacnet-bbmd-enabled";
        public const string BACnetConfigBBMDIp = "bacnet-bbmd-ip";
        public const string BACnetConfigBBMDTTL = "bacnet-bbmd-timetolive";
        public const string BACnetConfigGatewayInstanceId = "gateway-instance-id";
        public const string BACnetConfigPhysicalNetworkNumber = "physical-network-number";
        public const string BACnetConfigPhysicalNetworkNumberStatus = "physical-network-number-status";
        public const string BACnetConfigNoMatch = "No Match";


        public static Dictionary<string, string> AdapterGetMessageHeader = new()
        {
            { NxHeader.PropHeaderOp, "get" },
            { NxHeader.PropHeaderSync, "false" },
            { NxHeader.PropHeaderVersion, "1" }
        };

        public static Dictionary<string, string> AdapterSetMessageHeader = new()
        {
            { NxHeader.PropHeaderOp, "set" },
            { NxHeader.PropHeaderSync, "true" },
            { NxHeader.PropHeaderVersion, "1" }
        };
    }
}
