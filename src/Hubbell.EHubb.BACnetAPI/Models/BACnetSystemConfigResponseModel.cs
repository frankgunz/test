
using System.Collections.Generic;

namespace Hubbell.EHubb.BACnetAPI.Models
{
    public class BACnetSystemConfigResponseModel
    {
        public bool BacNetEnabled { get; set; }
        public int GatewayInstanceId { get; set; }
        public int PhysicalNetworkNumber { get; set; }
        public int PhysicalNetworkNumberStatus { get; set; }
        //public int DeviceInstanceSequenceStart { get; set; }
        //public int DeviceInstanceSequence { get; set; }
        public int RouterInstanceId { get; set; }
        public string RouterInstanceName { get; set; }
        public int Vnet { get; set; }
        public int Port { get; set; }
        public bool BbmdEnabled { get; set; }
        public string BbmdIpAddress { get; set; }
        public int BbmdTimeToLive { get; set; }
        //public int ZoneCount { get; set; }
        //public bool BlockDeviceIdsEnabled { get; set; }
        public string NXAdapterId { get; set; }
        public List<BacnetExportModel> ExportObjects { get; set; }
    }

    public class BacnetExportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }

}
