using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.Models
{
    public class BACnetZoneListResponseModel
    {
        public string Jsonrpc { get; set; }
        public List<BACnetZonelistParamsModel> Result { get; set; }
    }
    
    public class BACnetZonelistParamsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
