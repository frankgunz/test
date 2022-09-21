using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.Models
{
    public class BACnetZonePropertiesResponseModel
    {
        public string Jsonrpc { get; set; }
        public ZonePropertiesResult Result { get; set; }
    }

    public class ZonePropertiesResult
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public List<DetailsResult> Devices { get; set; }
        public List<DetailsResult> Presets { get; set; }
        public List<DetailsResult> Groups { get; set; }
    }

    public class DetailsResult
    {
        public string description { get; set; }
        public string name { get; set; }
        public string export_name { get; set; }
        public string device_type { get; set; }
        public string id { get; set; }
    }

}
