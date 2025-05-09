using System.Collections.Generic;
using System.Linq;

namespace SmartCityTransportMVC.Models.DataStructures
{
    public class RouteHashTable
    {
        private readonly Dictionary<string, double> table = new();

        public void AddTrafficData(string stop1, string stop2, double value)
        {
            var key = GenerateKey(stop1, stop2);
            table[key] = value;
        }

        public List<double> GetTrafficData(List<string> route)
        {
            var trafficList = new List<double>();
            for (int i = 0; i < route.Count - 1; i++)
            {
                var key = GenerateKey(route[i], route[i + 1]);
                trafficList.Add(table.ContainsKey(key) ? table[key] : 0.0);
            }
            return trafficList;
        }

        private string GenerateKey(string a, string b)
        {
            return string.Compare(a, b) < 0 ? $"{a}-{b}" : $"{b}-{a}";
        }
    }
}
