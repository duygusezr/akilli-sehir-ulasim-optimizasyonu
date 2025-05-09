using System.Collections.Generic;
using System.Linq;

namespace SmartCityTransportMVC.Models.DataStructures
{
    public class CityGraph
    {
        private Dictionary<string, (double lat, double lng)> Nodes = new();
        public Dictionary<string, List<(string to, double distance)>> Edges = new();

        public void AddStop(string stop, (double lat, double lng) coords)
        {
            Nodes[stop] = coords;
            if (!Edges.ContainsKey(stop))
                Edges[stop] = new();
        }

        public void AddRoute(string from, string to, double distance)
        {
            Edges[from].Add((to, distance));
            Edges[to].Add((from, distance));
        }

        public Dictionary<string, (double lat, double lng)> GetCoords() => Nodes;

        public List<List<string>> FindKShortestPaths(string start, string end, int k = 3)
        {
            var result = new List<List<string>>();
            var queue = new PriorityQueue<(List<string> Path, double Dist), double>();
            queue.Enqueue((new List<string> { start }, 0), 0);

            while (queue.Count > 0 && result.Count < k)
            {
                var (path, dist) = queue.Dequeue();
                var last = path.Last();

                if (last == end)
                {
                    result.Add(path);
                    continue;
                }

                if (!Edges.ContainsKey(last)) continue;

                foreach (var edge in Edges[last])
                {
                    var neighbor = edge.to;
                    var weight = edge.distance;

                    if (!path.Contains(neighbor)) 
                    {
                        var newPath = new List<string>(path) { neighbor };
                        queue.Enqueue((newPath, dist + weight), dist + weight);
                    }
                }
            }

            return result;
        }

 
        public double CalculateTotalDistance(List<string> path)
        {
            double total = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var from = path[i];
                var to = path[i + 1];
                var edge = Edges[from].FirstOrDefault(e => e.to == to);
                total += edge.distance;
            }
            return total;
        }
    }
}
