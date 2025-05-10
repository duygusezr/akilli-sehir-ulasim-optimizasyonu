using System.Collections.Generic;
using System.Linq;

namespace SmartCityTransportMVC.Models.DataStructures
{
    public class RouteLinkedList
    {
        public List<string> OptimizeRoute(List<string> route, List<double> trafficData)
        {
            return route.Zip(trafficData, (stop, traffic) => (stop, traffic))
                        .Where(x => x.traffic < 0.7)
                        .Select(x => x.stop)
                        .ToList();
        }
    }
}
