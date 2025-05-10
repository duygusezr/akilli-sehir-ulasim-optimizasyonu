using Microsoft.AspNetCore.Mvc;
using SmartCityTransportMVC.Models;
using SmartCityTransportMVC.Models.DataStructures;
using System.Diagnostics;

namespace SmartCityTransportMVC.Controllers
{
    public class HomeController : Controller
    {
        // Veri yapilari
        private static readonly CityGraph _cityGraph = new();
        private static readonly RouteHashTable _trafficTable = new();
        private static readonly RouteLinkedList _routeList = new();

        public IActionResult Index()
        {
            return View();
        }


       [HttpPost]
        public IActionResult FindRoute(RouteRequest request)
        {
            if (string.IsNullOrEmpty(request.Start) || string.IsNullOrEmpty(request.End))
            {
                ViewBag.Error = "Ba�lang�� ve biti� duraklar� girilmelidir.";
                return View("Index");
            }

            var alternativePaths = _cityGraph.FindKShortestPaths(request.Start, request.End, 3);

            if (alternativePaths.Count == 0)
            {
                ViewBag.Error = $"{request.Start} ile {request.End} aras�nda alternatif rota bulunamad�.";
                return View("Index");
            }

            var alternatives = new List<(List<string> Route, List<double> Traffic, double TotalTraffic, double Distance)>();

            foreach (var path in alternativePaths)
            {
                var traffic = _trafficTable.GetTrafficData(path);
                double totalTraffic = traffic.Sum();
                double distance = _cityGraph.CalculateTotalDistance(path);
                alternatives.Add((path, traffic, totalTraffic, distance));
            }

            var original = alternatives.First(); // Kullan�c�n�n g�rd��� ilk rota (s�ralamaya g�re)

            // %70 alt� t�m segmentlerden olu�an rotalar� filtrele
            var optimizables = alternatives.Where(a => a.Traffic.All(t => t < 0.7)).ToList();

            // En iyi rota
            var best = optimizables.Any()
                ? optimizables.OrderBy(a => a.TotalTraffic).First()
                : alternatives.OrderBy(a => a.TotalTraffic).First();

            bool isOptimized = optimizables.Any();
            bool areSame = best.Route.SequenceEqual(original.Route);

            var comparisonList = new List<(List<string> Route, List<double> Traffic, string Label, bool IsOptimized)>();

            comparisonList.Add((best.Route, best.Traffic, "1. Tercih", true));

            var second = alternatives
                .Where(a => !a.Route.SequenceEqual(best.Route))
                .OrderBy(a => a.TotalTraffic)
                .FirstOrDefault();

            if (second.Route != null)
            {
                comparisonList.Add((second.Route, second.Traffic, "2. Tercih", false));
            }


            ViewBag.RouteComparison = comparisonList;
            ViewBag.Coords = _cityGraph.GetCoords();
            ViewBag.Route = best.Route;
            ViewBag.Traffic = best.Traffic;

            return View("Index");
        }
        private void InitializeData()
        {
            var stops = new Dictionary<string, (double, double)>
        {
        { "Kad�k�y", (40.9923, 29.0295) },
        { "�sk�dar", (41.0254, 29.0146) },
        { "Be�ikta�", (41.0426, 29.0027) },
        { "Taksim", (41.0368, 28.9849) },
        { "Levent", (41.0811, 29.0108) },
        { "Bak�rk�y", (40.9744, 28.8718) },
        { "Mecidiyek�y", (41.0672, 28.9862) },
        { "�i�li", (41.0605, 28.9871) },
        { "Emin�n�", (41.0165, 28.9483) },
        { "Yenikap�", (41.0055, 28.9533) },
        { "Zincirlikuyu", (41.0705, 29.0139) },
        { "Topkap�", (41.0194, 28.9173) },
        { "Aksaray", (41.0129, 28.9514) },
        { "Fatih", (41.0183, 28.9485) },
        { "Kabata�", (41.0325, 28.9798) }
        };
        



            foreach (var stop in stops)
            {
                _cityGraph.AddStop(stop.Key, stop.Value);
            }
            AddRouteWithTraffic("Kad�k�y", "�sk�dar", 5.2);
            AddRouteWithTraffic("�sk�dar", "Be�ikta�", 4.8);
            AddRouteWithTraffic("Be�ikta�", "Taksim", 3.1);
            AddRouteWithTraffic("Taksim", "Levent", 6.7);
            AddRouteWithTraffic("Be�ikta�", "Levent", 5.3);
            AddRouteWithTraffic("Taksim", "Bak�rk�y", 12.5);
            AddRouteWithTraffic("Kad�k�y", "Bak�rk�y", 15.2);
            AddRouteWithTraffic("�sk�dar", "Taksim", 5.9);
            AddRouteWithTraffic("Levent", "Bak�rk�y", 14.3);
            AddRouteWithTraffic("Mecidiyek�y", "�i�li", 1.2);
            AddRouteWithTraffic("�i�li", "Taksim", 2.5);
            AddRouteWithTraffic("Mecidiyek�y", "Zincirlikuyu", 2.0);
            AddRouteWithTraffic("Zincirlikuyu", "Levent", 1.5);
            AddRouteWithTraffic("Topkap�", "Bak�rk�y", 5.5);
            AddRouteWithTraffic("Topkap�", "Emin�n�", 4.0);
            AddRouteWithTraffic("Emin�n�", "Taksim", 3.5);
            AddRouteWithTraffic("Emin�n�", "�sk�dar", 3.3);
            AddRouteWithTraffic("Yenikap�", "Topkap�", 2.8);
            AddRouteWithTraffic("Yenikap�", "Bak�rk�y", 4.0);
            AddRouteWithTraffic("Yenikap�", "Emin�n�", 2.4);
            AddRouteWithTraffic("Topkap�", "Fatih", 1.5);
            AddRouteWithTraffic("Fatih", "Aksaray", 1.2);
            AddRouteWithTraffic("Aksaray", "Yenikap�", 1.0);
            AddRouteWithTraffic("Aksaray", "Emin�n�", 1.7);
            AddRouteWithTraffic("Fatih", "Emin�n�", 1.3);
            AddRouteWithTraffic("Kabata�", "Be�ikta�", 1.2);
            AddRouteWithTraffic("Kabata�", "Taksim", 1.8);
            AddRouteWithTraffic("Kabata�", "Emin�n�", 2.1);
            AddRouteWithTraffic("�i�li", "Zincirlikuyu", 1.8);
            AddRouteWithTraffic("Mecidiyek�y", "Levent", 2.3);
            AddRouteWithTraffic("Aksaray", "Topkap�", 2.0);
            AddRouteWithTraffic("Kad�k�y", "Kabata�", 7.5);
            AddRouteWithTraffic("Kad�k�y", "Emin�n�", 6.2);
            AddRouteWithTraffic("Taksim", "Kabata�", 1.5);
            AddRouteWithTraffic("Kabata�", "�sk�dar", 4.0);
            AddRouteWithTraffic("Be�ikta�", "Levent", 6.1);
            AddRouteWithTraffic("Fatih", "Topkap�", 2.2);
            AddRouteWithTraffic("Topkap�", "Aksaray", 2.1);
            AddRouteWithTraffic("Fatih", "Mecidiyek�y", 4.5);  // ger�ek�i de�ilse bile test i�in olur
            AddRouteWithTraffic("Emin�n�", "Aksaray", 1.9);
            AddRouteWithTraffic("Fatih", "Kabata�", 3.0);
            AddRouteWithTraffic("Fatih", "Be�ikta�", 4.5);
            AddRouteWithTraffic("Fatih", "�i�li", 4.2);
            AddRouteWithTraffic("Aksaray", "Taksim", 4.0);
            AddRouteWithTraffic("Aksaray", "Be�ikta�", 4.8);
            AddRouteWithTraffic("Aksaray", "Levent", 6.2);
            AddRouteWithTraffic("Topkap�", "�i�li", 4.0);
            AddRouteWithTraffic("Topkap�", "Mecidiyek�y", 4.3);
            AddRouteWithTraffic("Yenikap�", "Be�ikta�", 5.2);
            AddRouteWithTraffic("Yenikap�", "Kabata�", 4.1);
            AddRouteWithTraffic("Yenikap�", "Zincirlikuyu", 6.3);
            AddRouteWithTraffic("Yenikap�", "Mecidiyek�y", 5.5);
            AddRouteWithTraffic("Emin�n�", "Levent", 5.7);
            AddRouteWithTraffic("Emin�n�", "Zincirlikuyu", 6.2);
            AddRouteWithTraffic("Kabata�", "�i�li", 3.8);
            AddRouteWithTraffic("Kabata�", "Zincirlikuyu", 4.2);
            AddRouteWithTraffic("Kad�k�y", "Levent", 9.3);       // Uzun ama alternatifli
            AddRouteWithTraffic("Kad�k�y", "�i�li", 8.8);
            AddRouteWithTraffic("Kad�k�y", "Taksim", 7.6);
            AddRouteWithTraffic("Kad�k�y", "Zincirlikuyu", 9.5);
            AddRouteWithTraffic("�sk�dar", "Zincirlikuyu", 6.9);
            AddRouteWithTraffic("�sk�dar", "�i�li", 6.4);
            AddRouteWithTraffic("�sk�dar", "Mecidiyek�y", 6.1);
            AddRouteWithTraffic("Zincirlikuyu", "Mecidiyek�y", 2.1);
            AddRouteWithTraffic("Zincirlikuyu", "Be�ikta�", 2.2);
            AddRouteWithTraffic("Kabata�", "Levent", 4.9);
            AddRouteWithTraffic("Levent", "�i�li", 2.0);
            AddRouteWithTraffic("Levent", "Mecidiyek�y", 1.9);
            AddRouteWithTraffic("Mecidiyek�y", "Be�ikta�", 2.8);
            AddRouteWithTraffic("�i�li", "Be�ikta�", 2.2);
            AddRouteWithTraffic("Taksim", "Mecidiyek�y", 2.5);
            AddRouteWithTraffic("Topkap�", "Kabata�", 5.2);
            AddRouteWithTraffic("Yenikap�", "Taksim", 4.5);
            AddRouteWithTraffic("Aksaray", "Kabata�", 3.9);
            AddRouteWithTraffic("Fatih", "Levent", 5.8);
            AddRouteWithTraffic("Fatih", "Zincirlikuyu", 6.1);
            AddRouteWithTraffic("�sk�dar", "Be�ikta�", 4.4); // alternatif mesafe
            AddRouteWithTraffic("Kad�k�y", "Mecidiyek�y", 7.8);
        }


        private void AddRouteWithTraffic(string from, string to, double distance)
        {
            _cityGraph.AddRoute(from, to, distance);
            double trafficValue = GetRandomTraffic($"{from}-{to}");
            _trafficTable.AddTrafficData(from, to, trafficValue);
        }

        private double GetRandomTraffic(string key)
        {
            int hash = Math.Abs(key.GetHashCode());
            return 0.3 + (hash % 60) / 100.0;
        }



    }
}
