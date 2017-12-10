using System;
using System.Collections.Generic;
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
            var viewModel = new AddressLookupViewModel();

            if (!ModelState.IsValid)
            {
                viewModel.Status = "Input Error: " + ModelState.Values.FirstOrDefault(e => e.Errors.Any())?.Errors.FirstOrDefault(x => !string.IsNullOrEmpty(x.ErrorMessage))?.ErrorMessage;
                return PartialView("~/Views/Partials/AddressLookup.cshtml", viewModel);
            }
            
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

                        viewModel.Format = "XML";
                        viewModel.RawOutput = rawXml;
                        viewModel.Status = parsedXml.Status;

                        if (parsedXml.Status == "OK")
                        {
                            viewModel.LocationData = new List<LocationData>();
                            var locationData = new LocationData
                            {
                                Route = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("route"))?.Long_name,
                                Country = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("country"))?.Long_name,
                                Premise = parsedXml.Result.Address_component.FirstOrDefault(x => x.Type.Contains("premise"))?.Long_name,
                                Longitude = parsedXml.Result.Geometry.Location.Lng,
                                Latitude = parsedXml.Result.Geometry.Location.Lat
                            };
                            viewModel.LocationData.Add(locationData);
                        }
                    }
                    catch (Exception ex)
                    {
                        viewModel.Status = "Error: " + ex.Message;
                    }
                    break;

                // JSON
                case Format.Json:
                    try
                    {
                        var rawJson = CallGoogle("json", address);
                        var parsedJson = JsonConvert.DeserializeObject<GeolocationJson>(rawJson);

                        viewModel.Format = "JSON";
                        viewModel.RawOutput = rawJson;
                        viewModel.Status = parsedJson.status;

                        if (parsedJson.status == "OK")
                        {
                            foreach (var result in parsedJson.results)
                            {
                                viewModel.LocationData = new List<LocationData>();
                                var locationData = new LocationData
                                {
                                    Route = result.address_components.FirstOrDefault(x => x.types.Contains("route"))?.long_name,
                                    Country = result.address_components.FirstOrDefault(x => x.types.Contains("country"))?.long_name,
                                    Premise = result.address_components.FirstOrDefault(x => x.types.Contains("premise"))?.long_name,
                                    Longitude = result.geometry.location.lng.ToString(CultureInfo.InvariantCulture),
                                    Latitude = result.geometry.location.lat.ToString(CultureInfo.InvariantCulture)
                                };
                                viewModel.LocationData.Add(locationData);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        viewModel.Status = "Error: " + ex.Message;
                    }
                    break;

                default:
                    viewModel.Status = "Input Error";
                    break;

            }

            return PartialView("~/Views/Partials/AddressLookup.cshtml", viewModel);
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