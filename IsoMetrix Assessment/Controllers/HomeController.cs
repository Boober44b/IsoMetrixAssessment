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
        private enum Format
        {
            Xml = 1,
            Json = 2
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddressLookup(FormData formData)
        {
            var address = formData.Address;
            Format formatOption;
            Enum.TryParse(formData.FormatOption, out formatOption);

            switch (formatOption)
            {
                // XML
                case Format.Xml:
                    try
                    {
                        var rawXml = CallGoogle("xml", address);

                        var serializer = new XmlSerializer(typeof(GeocodeResponse));
                        var memStream = new MemoryStream(Encoding.UTF8.GetBytes(rawXml));

                        var parsedXml = (GeocodeResponse)serializer.Deserialize(memStream);

                        ViewBag.Format = "XML";
                        ViewBag.RawOutput = rawXml;
                        ViewBag.Status = parsedXml.Status;

                        if (parsedXml.Status == "OK")
                        {
                            ViewBag.Route = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("route"))?.Long_name;
                            ViewBag.Country = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("country"))?.Long_name;
                            ViewBag.Premise = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("premise"))?.Long_name;
                            ViewBag.Longitude = parsedXml.Result.Geometry.Location.Lng;
                            ViewBag.Latitude = parsedXml.Result.Geometry.Location.Lat;
                        }
                    }
                    catch (Exception)
                    {
                        ViewBag.Status = "Error";
                    }
                    break;

                // JSON
                case Format.Json:
                    try
                    {
                        var rawJson = CallGoogle("json", address);
                        var parsedJson = JsonConvert.DeserializeObject<GeolocationJson>(rawJson);

                        ViewBag.Format = "JSON";
                        ViewBag.RawOutput = rawJson;
                        ViewBag.Status = parsedJson.status;

                        if (parsedJson.status == "OK")
                        {
                            ViewBag.Route = parsedJson.results.FirstOrDefault()?.address_components.FirstOrDefault(x => x.types.Contains("route"))?.long_name;
                            ViewBag.Country = parsedJson.results.FirstOrDefault()?.address_components.FirstOrDefault(x => x.types.Contains("country"))?.long_name;
                            ViewBag.Premise = parsedJson.results.FirstOrDefault()?.address_components.FirstOrDefault(x => x.types.Contains("premise"))?.long_name;
                            ViewBag.Longitude = parsedJson.results.FirstOrDefault()?.geometry.location.lng.ToString(CultureInfo.InvariantCulture);
                            ViewBag.Latitude = parsedJson.results.FirstOrDefault()?.geometry.location.lat.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    catch (Exception)
                    {
                        ViewBag.Status = "Error";
                    }
                    break;

                default:
                    ViewBag.Status = "Input Error";
                    break;
                    
            }

            return PartialView("~/Views/Partials/AddressLookup.cshtml");
        }

        private static string CallGoogle(string format, string address)
        {
            var requestUri = $"http://maps.googleapis.com/maps/api/geocode/{format}?address={Uri.EscapeDataString(address)}&sensor=false";
            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            return responseStream != null ? new StreamReader(responseStream).ReadToEnd() : string.Empty;
        }
    }

    
}