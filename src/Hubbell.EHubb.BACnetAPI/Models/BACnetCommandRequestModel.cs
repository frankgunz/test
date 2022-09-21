using System.Collections.Generic;

namespace Hubbell.EHubb.BACnetAPI.Models
{
    public class BACnetCommandRequestModel
    {
        public string Method { get; set; }
        public BACnetCommandParamsModel Params { get; set; }
        public int Id { get; set; }

    }

    public class BACnetCommandParamsModel
    {
        public string LookupKey { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public BACnetCommandSetModel Set { get; set; }
    }

    public class BACnetCommandSetModel
    { 
        public string Value { get; set; }
    }
}