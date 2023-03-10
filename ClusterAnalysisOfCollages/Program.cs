using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ClusterAnalysisOfCollages
{
    class Program
    {
        public static double calculataDistance(double x1, double y1, double x2, double y2)
            => Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

        public record Hospital (string name, (double x, double y) location, string district);
        public record District (string name, (double x, double y) location);

        static void Main(string[] args)
        {
            JArray hospitalsRaw = JArray.Parse(File.ReadAllText("Hospitals.json"));
            JObject districtsRaw = JObject.Parse(File.ReadAllText("DistrictsLocations.json"));

            IEnumerable<District> distritsQuery =
                from d in districtsRaw["Districts"]
                select new District((string)d["FullName"],
                                    ((double)d["Coordinates"][0], (double)d["Coordinates"][1]));

            IEnumerable<Hospital> hospitalsQuery =
                from h in hospitalsRaw
                let cords = h["geoData"]["coordinates"][0]
                select new Hospital((string)h["ShortName"],
                         ((double)cords[1], (double)cords[0]),
                         (string)h["ObjectAddress"][0]["AdmArea"]);

            uint correntPredictions = 0;
            foreach (var hospital in hospitalsQuery)
            {
                (string _, var (hx, hy), string bindedDistrictName) = hospital;
                var predictedDistrictName = distritsQuery
                    .GroupBy(
                        distr => calculataDistance(distr.location.x, distr.location.y, hx, hy),
                        distr => distr.name
                    )
                    .OrderBy(el => el.Key)
                    .First()
                    .First();
                Console.WriteLine("{0} is predicted district", predictedDistrictName);
                if (predictedDistrictName == bindedDistrictName)
                {
                    ++correntPredictions;
                }
            }
            double accuracy = (double)correntPredictions / hospitalsQuery.Count();
            Console.WriteLine("Accuracy is: {0}%", accuracy * 100);
        }
    }
}
