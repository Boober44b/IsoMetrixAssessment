using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsoMetrix_Assessment.Models
{
    public class AddressLookupViewModel
    {
        public string Status { get; set; }
        public string Format { get; set; }
        public string RawOutput { get; set; }
        public List<LocationData> LocationData { get; set; }
    }

    public class LocationData
    {
        public string Country { get; set; }
        public string Premise { get; set; }
        public string Route { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}