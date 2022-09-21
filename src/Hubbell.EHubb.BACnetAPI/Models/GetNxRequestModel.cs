using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.Models
{
    public class NxValueRequestModel
    {
        public string Method { get; set; }
        public NxValueParamsModel Params { get; set; }
        public int Id { get; set; }
    }
    public class NxValueParamsModel
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public NXSetParam Set { get; set; }
    }
    public class NXSetParam
    {
        public string present_value { get; set; }
    }
}
