using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Geometries;
using RoadSideAssistance.Models;
using RoadSideAssistance.Services;
using System.IO;

namespace RoadSideAssistance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RSAController : ControllerBase
    {
        
        private readonly IRoadSideAssistanceService _roadSideAssistanceService;
        private IRSAMemoryCache _cache;
        const string AssistantsCache = "AvailableAssistants";

        public IRSAMemoryCache MemoryCache 
        { 
            get { return _cache; }
            set { _cache = value; }
        }

        public RSAController(IMemoryCache cache)
        {
            _cache = new WrapperClsMemoryCache(cache);
            _roadSideAssistanceService = new Services.RoadSideAssistanceService(cache);
            
            
            List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>(AssistantsCache);
            //check if assistants data is loaded in cache. If not loaded, load the data
            if (assistantsDataCache == null)
            {
                var AssistantDataCache = LoadAssistants();
                _cache.Set(AssistantsCache, AssistantDataCache, new TimeSpan(24, 0, 0));
                assistantsDataCache = _cache.Get<List<Assistant>>(AssistantsCache);
            }
        }


        [HttpGet]
        [Route("GetNearestServiceTrucks")]
        public List<string> GetNearestServiceTrucks(double latitude, double longitude)
        {            

            Geolocation objLocation = new Geolocation();
            objLocation.Name = "Current Loc";
            objLocation.Latitude = latitude;
            objLocation.Longitude = longitude;           

            SortedSet<Assistant> assist = _roadSideAssistanceService.findNearestAssistants(objLocation, 5);

            List<string> lstNearestTrucks = assist.Select(a => a.Name).ToList();

            return lstNearestTrucks;
        }

        [HttpPost]
        [Route("ReserveServiceTruck")]
        public string ReserveServiceTruck(string CustomerName,double latitude, double longitude)
        {

            Geolocation objLocation = new Geolocation();
            objLocation.Name = "Current Loc";
            objLocation.Latitude = latitude;
            objLocation.Longitude = longitude;

            Customer objCustomer = new Customer();
            objCustomer.Name = CustomerName;
            objCustomer.Location = objLocation;
            objCustomer.IsServiceRequired = true;

            Assistant objAssistant = _roadSideAssistanceService.reserveAssistant(objCustomer, objLocation);

            _roadSideAssistanceService.updateAssistantLocation(objAssistant, objLocation);

            return objAssistant.Name;
        }


        [HttpPost]
        [Route("ReleaseServiceTruck")]
        public string ReleaseServiceTruck(string CustomerName, string TruckName)
        {



            Customer objCustomer = new Customer();
            objCustomer.Name = CustomerName;    
            objCustomer.IsServiceRequired = false;
            

            List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>(AssistantsCache);

            Assistant objAssistant = assistantsDataCache.Where(x => x.Name == TruckName).FirstOrDefault();

            _roadSideAssistanceService.releaseAssistant(objCustomer, objAssistant);

            return "Success";
        }

        [HttpGet]
        [Route("GetAllAssistants")]
        public object GetAllAssistants()
        {

          
            List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>(AssistantsCache);
          
            var assistants = assistantsDataCache
                                .Select(x=> new { x.Name, x.IsAvailable, x.Customer })
                                .Distinct()
                                .ToList();

            return assistants;
        }

        [HttpGet]
        [Route("GetAllAssignedAssistants")]
        public object GetAllAssignedAssistants()
        {


            List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>(AssistantsCache);

            var assistants = assistantsDataCache
                                .Select(x => new { x.Name, x.IsAvailable, x.Customer })
                                .Where(x=> x.IsAvailable==false)
                                .Distinct()
                                .ToList();

            return assistants;
        }

        private List<Assistant> LoadAssistants()
        {
            List<Assistant> regionalAssistants = new List<Assistant>();
            try
            {
                List<Geolocation> geolocations = null;
                //load data from csv file
                geolocations = System.IO.File.ReadAllLines(".\\Data\\sampledata.csv")
                                               .Skip(1)
                                               .Select(v => Geolocation.FromCsv(v))
                                               .ToList();

                foreach (var item in geolocations)
                {
                    //4326 below refers to WGS 84, a standard used in GPS and other geographic systems
                    Assistant assistant = new Assistant();
                    assistant.Location = new Point(item.Latitude, item.Longitude) { SRID = 4326 };
                    assistant.Name = item.Name;
                    assistant.IsAvailable = true;
                    assistant.Customer = "";

                    regionalAssistants.Add(assistant);
                }
            }
            catch
            {
                //consuming the exception
            }
            return regionalAssistants;
        }

    }
}
