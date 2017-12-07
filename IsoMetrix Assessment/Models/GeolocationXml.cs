using System.Collections.Generic;
using System.Xml.Serialization;

namespace IsoMetrix_Assessment.Models
{
    public class GeolocationXml
    {
        public List<Result2> result { get; set; }
        public string status { get; set; }
    }
    [XmlRoot(ElementName = "address_component")]
    public class Address_component
    {
        [XmlElement(ElementName = "long_name")]
        public string Long_name { get; set; }
        [XmlElement(ElementName = "short_name")]
        public string Short_name { get; set; }
        [XmlElement(ElementName = "type")]
        public List<string> Type { get; set; }
    }

    [XmlRoot(ElementName = "location")]
    public class Location2
    {
        [XmlElement(ElementName = "lat")]
        public string Lat { get; set; }
        [XmlElement(ElementName = "lng")]
        public string Lng { get; set; }
    }

    [XmlRoot(ElementName = "southwest")]
    public class Southwest2
    {
        [XmlElement(ElementName = "lat")]
        public string Lat { get; set; }
        [XmlElement(ElementName = "lng")]
        public string Lng { get; set; }
    }

    [XmlRoot(ElementName = "northeast")]
    public class Northeast2
    {
        [XmlElement(ElementName = "lat")]
        public string Lat { get; set; }
        [XmlElement(ElementName = "lng")]
        public string Lng { get; set; }
    }

    [XmlRoot(ElementName = "viewport")]
    public class Viewport2
    {
        [XmlElement(ElementName = "southwest")]
        public Southwest2 Southwest { get; set; }
        [XmlElement(ElementName = "northeast")]
        public Northeast2 Northeast { get; set; }
    }

    [XmlRoot(ElementName = "geometry")]
    public class Geometry2
    {
        [XmlElement(ElementName = "location")]
        public Location2 Location { get; set; }
        [XmlElement(ElementName = "location_type")]
        public string Location_type { get; set; }
        [XmlElement(ElementName = "viewport")]
        public Viewport2 Viewport { get; set; }
    }

    [XmlRoot(ElementName = "result")]
    public class Result2
    {
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "formatted_address")]
        public string Formatted_address { get; set; }
        [XmlElement(ElementName = "address_component")]
        public List<Address_component> Address_component { get; set; }
        [XmlElement(ElementName = "geometry")]
        public Geometry2 Geometry { get; set; }
        [XmlElement(ElementName = "place_id")]
        public string Place_id { get; set; }
    }

    [XmlRoot(ElementName = "GeocodeResponse")]
    public class GeocodeResponse
    {
        [XmlElement(ElementName = "status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "result")]
        public Result2 Result { get; set; }
    }
}