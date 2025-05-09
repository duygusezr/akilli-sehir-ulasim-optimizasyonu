using Microsoft.AspNetCore.Mvc;
using SmartCityTransportMVC.Models;
using SmartCityTransportMVC.Models.DataStructures;
using System.Diagnostics;

namespace SmartCityTransportMVC.Controllers
{
    public class HomeController : Controller
    {

        private static readonly CityGraph _cityGraph = new();
        private static readonly RouteHashTable _trafficTable = new();


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void InitializeData()
        {
            var stops = new Dictionary<string, (double, double)>
        {
        { "Kadýköy", (40.9923, 29.0295) },
        { "Üsküdar", (41.0254, 29.0146) },
        { "Beþiktaþ", (41.0426, 29.0027) },
        { "Taksim", (41.0368, 28.9849) },
        { "Levent", (41.0811, 29.0108) },
        { "Bakýrköy", (40.9744, 28.8718) },
        { "Mecidiyeköy", (41.0672, 28.9862) },
        { "Þiþli", (41.0605, 28.9871) },
        { "Eminönü", (41.0165, 28.9483) },
        { "Yenikapý", (41.0055, 28.9533) },
        { "Zincirlikuyu", (41.0705, 29.0139) },
        { "Topkapý", (41.0194, 28.9173) },
        { "Aksaray", (41.0129, 28.9514) },
        { "Fatih", (41.0183, 28.9485) },
        { "Kabataþ", (41.0325, 28.9798) }
        };
        



            foreach (var stop in stops)
            {
                _cityGraph.AddStop(stop.Key, stop.Value);
            }
            AddRouteWithTraffic("Kadýköy", "Üsküdar", 5.2);
            AddRouteWithTraffic("Üsküdar", "Beþiktaþ", 4.8);
            AddRouteWithTraffic("Beþiktaþ", "Taksim", 3.1);
            AddRouteWithTraffic("Taksim", "Levent", 6.7);
            AddRouteWithTraffic("Beþiktaþ", "Levent", 5.3);
            AddRouteWithTraffic("Taksim", "Bakýrköy", 12.5);
            AddRouteWithTraffic("Kadýköy", "Bakýrköy", 15.2);
            AddRouteWithTraffic("Üsküdar", "Taksim", 5.9);
            AddRouteWithTraffic("Levent", "Bakýrköy", 14.3);
            AddRouteWithTraffic("Mecidiyeköy", "Þiþli", 1.2);
            AddRouteWithTraffic("Þiþli", "Taksim", 2.5);
            AddRouteWithTraffic("Mecidiyeköy", "Zincirlikuyu", 2.0);
            AddRouteWithTraffic("Zincirlikuyu", "Levent", 1.5);
            AddRouteWithTraffic("Topkapý", "Bakýrköy", 5.5);
            AddRouteWithTraffic("Topkapý", "Eminönü", 4.0);
            AddRouteWithTraffic("Eminönü", "Taksim", 3.5);
            AddRouteWithTraffic("Eminönü", "Üsküdar", 3.3);
            AddRouteWithTraffic("Yenikapý", "Topkapý", 2.8);
            AddRouteWithTraffic("Yenikapý", "Bakýrköy", 4.0);
            AddRouteWithTraffic("Yenikapý", "Eminönü", 2.4);
            AddRouteWithTraffic("Topkapý", "Fatih", 1.5);
            AddRouteWithTraffic("Fatih", "Aksaray", 1.2);
            AddRouteWithTraffic("Aksaray", "Yenikapý", 1.0);
            AddRouteWithTraffic("Aksaray", "Eminönü", 1.7);
            AddRouteWithTraffic("Fatih", "Eminönü", 1.3);
            AddRouteWithTraffic("Kabataþ", "Beþiktaþ", 1.2);
            AddRouteWithTraffic("Kabataþ", "Taksim", 1.8);
            AddRouteWithTraffic("Kabataþ", "Eminönü", 2.1);
            AddRouteWithTraffic("Þiþli", "Zincirlikuyu", 1.8);
            AddRouteWithTraffic("Mecidiyeköy", "Levent", 2.3);
            AddRouteWithTraffic("Aksaray", "Topkapý", 2.0);
            AddRouteWithTraffic("Kadýköy", "Kabataþ", 7.5);
            AddRouteWithTraffic("Kadýköy", "Eminönü", 6.2);
            AddRouteWithTraffic("Taksim", "Kabataþ", 1.5);
            AddRouteWithTraffic("Kabataþ", "Üsküdar", 4.0);
            AddRouteWithTraffic("Beþiktaþ", "Levent", 6.1);
            AddRouteWithTraffic("Fatih", "Topkapý", 2.2);
            AddRouteWithTraffic("Topkapý", "Aksaray", 2.1);
            AddRouteWithTraffic("Fatih", "Mecidiyeköy", 4.5);  // gerçekçi deðilse bile test için olur
            AddRouteWithTraffic("Eminönü", "Aksaray", 1.9);
            AddRouteWithTraffic("Fatih", "Kabataþ", 3.0);
            AddRouteWithTraffic("Fatih", "Beþiktaþ", 4.5);
            AddRouteWithTraffic("Fatih", "Þiþli", 4.2);
            AddRouteWithTraffic("Aksaray", "Taksim", 4.0);
            AddRouteWithTraffic("Aksaray", "Beþiktaþ", 4.8);
            AddRouteWithTraffic("Aksaray", "Levent", 6.2);
            AddRouteWithTraffic("Topkapý", "Þiþli", 4.0);
            AddRouteWithTraffic("Topkapý", "Mecidiyeköy", 4.3);
            AddRouteWithTraffic("Yenikapý", "Beþiktaþ", 5.2);
            AddRouteWithTraffic("Yenikapý", "Kabataþ", 4.1);
            AddRouteWithTraffic("Yenikapý", "Zincirlikuyu", 6.3);
            AddRouteWithTraffic("Yenikapý", "Mecidiyeköy", 5.5);
            AddRouteWithTraffic("Eminönü", "Levent", 5.7);
            AddRouteWithTraffic("Eminönü", "Zincirlikuyu", 6.2);
            AddRouteWithTraffic("Kabataþ", "Þiþli", 3.8);
            AddRouteWithTraffic("Kabataþ", "Zincirlikuyu", 4.2);
            AddRouteWithTraffic("Kadýköy", "Levent", 9.3);       // Uzun ama alternatifli
            AddRouteWithTraffic("Kadýköy", "Þiþli", 8.8);
            AddRouteWithTraffic("Kadýköy", "Taksim", 7.6);
            AddRouteWithTraffic("Kadýköy", "Zincirlikuyu", 9.5);
            AddRouteWithTraffic("Üsküdar", "Zincirlikuyu", 6.9);
            AddRouteWithTraffic("Üsküdar", "Þiþli", 6.4);
            AddRouteWithTraffic("Üsküdar", "Mecidiyeköy", 6.1);
            AddRouteWithTraffic("Zincirlikuyu", "Mecidiyeköy", 2.1);
            AddRouteWithTraffic("Zincirlikuyu", "Beþiktaþ", 2.2);
            AddRouteWithTraffic("Kabataþ", "Levent", 4.9);
            AddRouteWithTraffic("Levent", "Þiþli", 2.0);
            AddRouteWithTraffic("Levent", "Mecidiyeköy", 1.9);
            AddRouteWithTraffic("Mecidiyeköy", "Beþiktaþ", 2.8);
            AddRouteWithTraffic("Þiþli", "Beþiktaþ", 2.2);
            AddRouteWithTraffic("Taksim", "Mecidiyeköy", 2.5);
            AddRouteWithTraffic("Topkapý", "Kabataþ", 5.2);
            AddRouteWithTraffic("Yenikapý", "Taksim", 4.5);
            AddRouteWithTraffic("Aksaray", "Kabataþ", 3.9);
            AddRouteWithTraffic("Fatih", "Levent", 5.8);
            AddRouteWithTraffic("Fatih", "Zincirlikuyu", 6.1);
            AddRouteWithTraffic("Üsküdar", "Beþiktaþ", 4.4); // alternatif mesafe
            AddRouteWithTraffic("Kadýköy", "Mecidiyeköy", 7.8);
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
