using System.Collections.Generic;

namespace Hubbell.EHubb.BACnetAPI.Models
{

    public class BACnetCommandResponseModel
    {
        public string Jsonrpc { get; set; }
        public Result Result { get; set; }
        public int Id { get; set; }       
    }

    public class Result
    {
        public string Value { get; set; }
        public string present_value { get; set; }
        public string Disposition { get; set; }
    }
}
