using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;
using IsoMetrix_Assessment.Models;
using Newtonsoft.Json;

namespace IsoMetrix_Assessment.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddressLookup(FormData formData)
        {
            var address = formData.Address;

            string requestUri;
            WebRequest request;
            WebResponse response;

            switch (formData.FormatOption)
            {
                // XML
                case "1":
                    
                    requestUri = $"http://maps.googleapis.com/maps/api/geocode/xml?address={Uri.EscapeDataString(address)}&sensor=false";
                    request = WebRequest.Create(requestUri);
                    response = request.GetResponse();
                    var rawXml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    ViewBag.Format = "XML";
                    ViewBag.RawOutput = rawXml;

                    var serializer = new XmlSerializer(typeof(GeocodeResponse));
                    var memStream = new MemoryStream(Encoding.UTF8.GetBytes(rawXml));

                    var parsedXml = (GeocodeResponse)serializer.Deserialize(memStream);

                    ViewBag.Status = parsedXml.Status;

                    if (parsedXml.Status == "OK")
                    {
                        ViewBag.Route = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("route"))?.Long_name;
                        ViewBag.Country = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("country"))?.Long_name;
                        ViewBag.Premise = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("premise"))?.Long_name;
                        ViewBag.Longitude = parsedXml.Result.Geometry.Location.Lng;
                        ViewBag.Latitude = parsedXml.Result.Geometry.Location.Lat;
                    }
                    break;

                // JSON
                case "2":

                    requestUri = $"http://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&sensor=false";
                    request = WebRequest.Create(requestUri);
                    response = request.GetResponse();
                    var rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    ViewBag.Format = "JSON";
                    ViewBag.RawOutput = rawJson;

                    var parsedJson = JsonConvert.DeserializeObject<GeolocationJson>(rawJson);

                    ViewBag.Status = parsedJson.status;

                    if (parsedJson.status == "OK")
                    {
                        ViewBag.Route = parsedJson.results.FirstOrDefault()?.address_components.FirstOrDefault(x => x.types.Contains("route"))?.long_name;
                        ViewBag.Country = parsedJson.results.FirstOrDefault()?.address_components.FirstOrDefault(x => x.types.Contains("country"))?.long_name;
                        ViewBag.Premise = parsedJson.results.FirstOrDefault()?.address_components.FirstOrDefault(x => x.types.Contains("premise"))?.long_name;
                        ViewBag.Longitude = parsedJson.results.FirstOrDefault()?.geometry.location.lng.ToString(CultureInfo.InvariantCulture);
                        ViewBag.Latitude = parsedJson.results.FirstOrDefault()?.geometry.location.lat.ToString(CultureInfo.InvariantCulture);
                    }
                    break;

                default: break;
                    
            }

            return PartialView("~/Views/Partials/AddressLookup.cshtml");
        }
    }
}