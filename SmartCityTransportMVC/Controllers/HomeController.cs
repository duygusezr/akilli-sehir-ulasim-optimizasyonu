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

        // Statik kurulum (ilk �al��t�rmada sadece 1 kez yap�ls�n)
        private static bool isInitialized = false;

        public HomeController()
        {
            if (!isInitialized)
            {
                InitializeData();
                isInitialized = true;
            }
        }
        
        
        public IActionResult Index()
        {
            return View();
        }


       [HttpPost]
        public IActionResult FindRoute(RouteRequest request)
        {
            if (string.IsNullOrEmpty(request.Start) || string.IsNullOrEmpty(request.End))
            {
                ViewBag.Error = "Baslangic ve bitis duraklari girilmelidir.";
                return View("Index");
            }

            var alternativePaths = _cityGraph.FindKShortestPaths(request.Start, request.End, 3);

            if (alternativePaths.Count == 0)
            {
                ViewBag.Error = $"{request.Start} ile {request.End} arasinda alternatif rota bulunamadi.";
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
        { "Kadikoy", (40.9923, 29.0295) },
        { "Uskudar", (41.0254, 29.0146) },
        { "Besiktas", (41.0426, 29.0027) },
        { "Taksim", (41.0368, 28.9849) },
        { "Levent", (41.0811, 29.0108) },
        { "Bakirkoy", (40.9744, 28.8718) },
        { "Mecidiyekoy", (41.0672, 28.9862) },
        { "Sisli", (41.0605, 28.9871) },
        { "Eminonu", (41.0165, 28.9483) },
        { "Yenikapi", (41.0055, 28.9533) },
        { "Zincirlikuyu", (41.0705, 29.0139) },
        { "Topkapi", (41.0194, 28.9173) },
        { "Aksaray", (41.0129, 28.9514) },
        { "Fatih", (41.0183, 28.9485) },
        { "Kabatas", (41.0325, 28.9798) }
        };




            foreach (var stop in stops)
            {
                _cityGraph.AddStop(stop.Key, stop.Value);
            }
            AddRouteWithTraffic("Kadikoy", "Uskudar", 5.2);
            AddRouteWithTraffic("Uskudar", "Besiktas", 4.8);
            AddRouteWithTraffic("Besiktas", "Taksim", 3.1);
            AddRouteWithTraffic("Taksim", "Levent", 6.7);
            AddRouteWithTraffic("Besiktas", "Levent", 5.3);
            AddRouteWithTraffic("Taksim", "Bakirkoy", 12.5);
            AddRouteWithTraffic("Kadikoy", "Bakirkoy", 15.2);
            AddRouteWithTraffic("Uskudar", "Taksim", 5.9);
            AddRouteWithTraffic("Levent", "Bakirkoy", 14.3);
            AddRouteWithTraffic("Mecidiyekoy", "Sisli", 1.2);
            AddRouteWithTraffic("Sisli", "Taksim", 2.5);
            AddRouteWithTraffic("Mecidiyekoy", "Zincirlikuyu", 2.0);
            AddRouteWithTraffic("Zincirlikuyu", "Levent", 1.5);
            AddRouteWithTraffic("Topkapi", "Bakirkoy", 5.5);
            AddRouteWithTraffic("Topkapi", "Eminonu", 4.0);
            AddRouteWithTraffic("Eminonu", "Taksim", 3.5);
            AddRouteWithTraffic("Eminonu", "Uskudar", 3.3);
            AddRouteWithTraffic("Yenikapi", "Topkapi", 2.8);
            AddRouteWithTraffic("Yenikapi", "Bakirkoy", 4.0);
            AddRouteWithTraffic("Yenikapi", "Eminonu", 2.4);
            AddRouteWithTraffic("Topkapi", "Fatih", 1.5);
            AddRouteWithTraffic("Fatih", "Aksaray", 1.2);
            AddRouteWithTraffic("Aksaray", "Yenikapi", 1.0);
            AddRouteWithTraffic("Aksaray", "Eminonu", 1.7);
            AddRouteWithTraffic("Fatih", "Eminonu", 1.3);
            AddRouteWithTraffic("Kabatas", "Besiktas", 1.2);
            AddRouteWithTraffic("Kabatas", "Taksim", 1.8);
            AddRouteWithTraffic("Kabatas", "Eminonu", 2.1);
            AddRouteWithTraffic("Sisli", "Zincirlikuyu", 1.8);
            AddRouteWithTraffic("Mecidiyekoy", "Levent", 2.3);
            AddRouteWithTraffic("Aksaray", "Topkapi", 2.0);
            AddRouteWithTraffic("Kadikoy", "Kabatas", 7.5);
            AddRouteWithTraffic("Kadikoy", "Eminonu", 6.2);
            AddRouteWithTraffic("Taksim", "Kabatas", 1.5);
            AddRouteWithTraffic("Kabatas", "Uskudar", 4.0);
            AddRouteWithTraffic("Besiktas", "Levent", 6.1);
            AddRouteWithTraffic("Fatih", "Topkapi", 2.2);
            AddRouteWithTraffic("Topkapi", "Aksaray", 2.1);
            AddRouteWithTraffic("Fatih", "Mecidiyekoy", 4.5);
            AddRouteWithTraffic("Eminonu", "Aksaray", 1.9);
            AddRouteWithTraffic("Fatih", "Kabatas", 3.0);
            AddRouteWithTraffic("Fatih", "Besiktas", 4.5);
            AddRouteWithTraffic("Fatih", "Sisli", 4.2);
            AddRouteWithTraffic("Aksaray", "Taksim", 4.0);
            AddRouteWithTraffic("Aksaray", "Besiktas", 4.8);
            AddRouteWithTraffic("Aksaray", "Levent", 6.2);
            AddRouteWithTraffic("Topkapi", "Sisli", 4.0);
            AddRouteWithTraffic("Topkapi", "Mecidiyekoy", 4.3);
            AddRouteWithTraffic("Yenikapi", "Besiktas", 5.2);
            AddRouteWithTraffic("Yenikapi", "Kabatas", 4.1);
            AddRouteWithTraffic("Yenikapi", "Zincirlikuyu", 6.3);
            AddRouteWithTraffic("Yenikapi", "Mecidiyekoy", 5.5);
            AddRouteWithTraffic("Eminonu", "Levent", 5.7);
            AddRouteWithTraffic("Eminonu", "Zincirlikuyu", 6.2);
            AddRouteWithTraffic("Kabatas", "Sisli", 3.8);
            AddRouteWithTraffic("Kabatas", "Zincirlikuyu", 4.2);
            AddRouteWithTraffic("Kadikoy", "Levent", 9.3);
            AddRouteWithTraffic("Kadikoy", "Sisli", 8.8);
            AddRouteWithTraffic("Kadikoy", "Taksim", 7.6);
            AddRouteWithTraffic("Kadikoy", "Zincirlikuyu", 9.5);
            AddRouteWithTraffic("Uskudar", "Zincirlikuyu", 6.9);
            AddRouteWithTraffic("Uskudar", "Sisli", 6.4);
            AddRouteWithTraffic("Uskudar", "Mecidiyekoy", 6.1);
            AddRouteWithTraffic("Zincirlikuyu", "Mecidiyekoy", 2.1);
            AddRouteWithTraffic("Zincirlikuyu", "Besiktas", 2.2);
            AddRouteWithTraffic("Kabatas", "Levent", 4.9);
            AddRouteWithTraffic("Levent", "Sisli", 2.0);
            AddRouteWithTraffic("Levent", "Mecidiyekoy", 1.9);
            AddRouteWithTraffic("Mecidiyekoy", "Besiktas", 2.8);
            AddRouteWithTraffic("Sisli", "Besiktas", 2.2);
            AddRouteWithTraffic("Taksim", "Mecidiyekoy", 2.5);
            AddRouteWithTraffic("Topkapi", "Kabatas", 5.2);
            AddRouteWithTraffic("Yenikapi", "Taksim", 4.5);
            AddRouteWithTraffic("Aksaray", "Kabatas", 3.9);
            AddRouteWithTraffic("Fatih", "Levent", 5.8);
            AddRouteWithTraffic("Fatih", "Zincirlikuyu", 6.1);
            AddRouteWithTraffic("Uskudar", "Besiktas", 4.4);
            AddRouteWithTraffic("Kadikoy", "Mecidiyekoy", 7.8);
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
