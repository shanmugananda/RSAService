using RoadSideAssistance.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using System.Collections.Immutable;
using Microsoft.Extensions.Caching.Memory;

namespace RoadSideAssistance.Services
{
    public class RoadSideAssistanceService : IRoadSideAssistanceService
    {
        private List<Assistant> regionalAssistants = new List<Assistant>();
        private IMemoryCache _cache;
        public RoadSideAssistanceService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// This method returns a collection of roadside assistants ordered by their distance from the input geo location
        /// </summary>
        /// <param name="geolocation">geolocation from which to search for assistants</param>
        /// <param name="limit">the number of assistants to return</param>
        /// <returns></returns>
        public SortedSet<Assistant> findNearestAssistants(Geolocation geolocation, int limit)
        {
            SortedSet<Assistant> assistants = new SortedSet<Assistant>(new AssistantComparer());

            try
            {
                //4326 below refers to WGS 84, a standard used in GPS and other geographic systems
                //as specified in https://learn.microsoft.com/en-us/ef/core/modeling/spatial
                var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

                var currentLocation = gf.CreatePoint(new NetTopologySuite.Geometries.Coordinate(geolocation.Latitude, geolocation.Longitude));

                List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>("AvailableAssistants");

                //get the list of assistants near the customer location
                var nearestassistants = assistantsDataCache.Where(k => k.IsAvailable == true).
                                                OrderBy(a => a.Location.Distance(gf.CreateGeometry(currentLocation))).Take(limit);

                assistants.UnionWith(nearestassistants);
            }
            catch
            {
                //consuming the exception
            }
            finally
            {

            }
            return assistants;
        }

        /// <summary>
        ///  This method releases an assistant either after they have completed work, or the customer no longer needs help
        /// </summary>
        /// <param name="customer">Represents a Geico customer</param>
        /// <param name="assistant">represents the roadside assistance service provider</param>
        public void releaseAssistant(Customer customer, Assistant assistant)
        {
            try
            {
                //if the customer doesn't need the assistance, update assistant as available and release assistant
                if (!customer.IsServiceRequired)
                {
                    List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>("AvailableAssistants");

                    assistant.Customer = "";
                    assistant.IsAvailable = true;

                    _cache.Set("AvailableAssistants", assistantsDataCache, new TimeSpan(24, 0, 0));

                }
            }
            catch
            {
                //consuming the exception
            }
        }

        /// <summary>
        /// This method reserves an assistant for a Geico customer that is stranded on the roadside due to a disabled vehicle
        /// </summary>
        /// <param name="customer">Represents a Geico customer</param>
        /// <param name="customerLocation">Location of the customer</param>
        /// <returns></returns>
        public Assistant reserveAssistant(Customer customer, Geolocation customerLocation)
        {
            
            Assistant assistant = null;

            try
            {
                //check if the customer needs service.
                //the customer object also has a property "InsuranceNumber".
                //This InsuranceNumber can be used to validate if the customer is valid or not before providing service. It was not used in this code as we need a service to check validity.
                if (customer.IsServiceRequired)
                {

                    List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>("AvailableAssistants");
                    SortedSet<Assistant> availableAssistants = findNearestAssistants(customerLocation, 1);
                    //get the nearest service assistant
                    assistant = availableAssistants.FirstOrDefault();

                    //update the assistant's location as the customer's location and reserve the assistant
                    assistant.Location = new Point(customer.Location.Latitude, customer.Location.Longitude) { SRID = 4326 };
                    assistant.Customer = customer.Name;
                    assistant.IsAvailable = false;

                    _cache.Set("AvailableAssistants", assistantsDataCache, new TimeSpan(24, 0, 0));
                }
            }
            catch
            {
                //consuming the exception
            }

            return assistant;
            
        }

        /// <summary>
        /// This method is used to update the location of the roadside assistance service provider
        /// </summary>
        /// <param name="assistant">represents the roadside assistance service provider</param>
        /// <param name="assistantLocation">represents the location of the roadside assistant</param>
        public void updateAssistantLocation(Assistant assistant, Geolocation assistantLocation)
        {
            try
            {
                List<Assistant> assistantsDataCache = _cache.Get<List<Assistant>>("AvailableAssistants");

                assistant.Location = new Point(assistantLocation.Latitude, assistantLocation.Longitude) { SRID = 4326 };


                _cache.Set("AvailableAssistants", assistantsDataCache, new TimeSpan(24, 0, 0));
            }
            catch
            {
                //consuming the exception
            }
        }
        
      
    }
}
