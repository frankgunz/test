using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubbell.EHubb.BACnetAPI.Models
{ //HP-157131
    public class ZoneListConfigResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ZoneNumber { get; set; }
        public int port { get; set; }
        public int ExtZoneId { get; set; }
        public string Description { get; set; }
        public Guid AreaId { get; set; }
    }
}



