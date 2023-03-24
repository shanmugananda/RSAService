using RoadSideAssistance.Models;

namespace RoadSideAssistance.Services
{
        public interface IRoadSideAssistanceService
    {

        /// <summary>
        /// This method is used to update the location of the roadside assistance service provider
        /// </summary>
        /// <param name="assistant">represents the roadside assistance service provider</param>
        /// <param name="assistantLocation">represents the location of the roadside assistant</param>
        void updateAssistantLocation(Assistant assistant, Geolocation assistantLocation);

        /// <summary>
        /// This method returns a collection of roadside assistants ordered by their distance from the input geo location
        /// </summary>
        /// <param name="geolocation">geolocation from which to search for assistants</param>
        /// <param name="limit">the number of assistants to return</param>
        /// <returns></returns>
        SortedSet<Assistant> findNearestAssistants(Geolocation geolocation, int limit);

        /// <summary>
        /// This method reserves an assistant for a Geico customer that is stranded on the roadside due to a disabled vehicle
        /// </summary>
        /// <param name="customer">Represents a Geico customer</param>
        /// <param name="customerLocation">Location of the customer</param>
        /// <returns></returns>
        Assistant reserveAssistant(Customer customer, Geolocation customerLocation);

        /// <summary>
        ///  This method releases an assistant either after they have completed work, or the customer no longer needs help
        /// </summary>
        /// <param name="customer">Represents a Geico customer</param>
        /// <param name="assistant">represents the roadside assistance service provider</param>
        void releaseAssistant(Customer customer, Assistant assistant);


    }
}
