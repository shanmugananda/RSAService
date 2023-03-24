namespace RoadSideAssistance.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VehicleType { get; set; }
        public string InsuranceNumber { get; set; }
        public bool IsServiceRequired { get; set; }

        public Geolocation Location { get; set; }
    }
}
