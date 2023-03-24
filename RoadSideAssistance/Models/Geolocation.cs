
namespace RoadSideAssistance.Models
{
    public class Geolocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public static Geolocation FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            Geolocation location = new Geolocation();
            location.Latitude = double.Parse(values[0]);
            location.Longitude = double.Parse(values[1]);
            location.Name = Convert.ToString(values[2]);
            location.Address = Convert.ToString(values[3]);            
            return location;
        }

    }
}
