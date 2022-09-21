using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.Models
{
    public class BACnetGetNxValueResponseModel
    {
        public GetNxValueResult getNxValueResult { get; set; }
        public int id { get; set; }
    }
    public class GetNxValueResult
    {
        public string value { get; set; }
    }
}
