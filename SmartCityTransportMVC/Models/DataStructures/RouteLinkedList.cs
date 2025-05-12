using System.Collections.Generic;

namespace SmartCityTransportMVC.Models.DataStructures
{
    public class RouteLinkedList
    {
        public LinkedList<string> OptimizeRoute(List<string> route, List<double> trafficData)
        {
            // Bağlı listeye rota verilerini aktar
            var linkedRoute = new LinkedList<string>(route);

            var current = linkedRoute.First;
            int index = 0;

            while (current != null && current.Next != null && index < trafficData.Count)
            {
                var next = current.Next;
                double traffic = trafficData[index];

                // Eğer trafik yoğunluğu 0.7 veya üzeriyse, sonraki durağı sil
                if (traffic >= 0.7)
                {
                    linkedRoute.Remove(next);
                }
                else
                {
                    current = current.Next;
                }

                index++;
            }

            return linkedRoute;
        }
    }
}
