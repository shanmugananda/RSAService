using NetTopologySuite.Geometries;

namespace RoadSideAssistance.Models
{
    public class Assistant:IComparable
    {
        public string Name { get; set; }
        //public Geolocation Location { get; set; }
        public bool IsAvailable { get; set; }
        public Point Location { get; set; }

        public string Customer { get; set; }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }
    }

    public class AssistantComparer : IComparer<Assistant>
    {
        public int Compare(Assistant x,Assistant y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
