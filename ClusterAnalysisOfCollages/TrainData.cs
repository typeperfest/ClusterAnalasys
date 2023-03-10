using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClusterAnalysisOfCollages
{
    public static class DistrictsDictionaty
    {
        private static Dictionary<string, List<bool>> namesCodes;

        public static void readFromFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            var line = sr.ReadLine();
            while (line != null)
            {
                string districtName = line.Split(':')[0];
                string districtCodeString = line.Split(':')[1];
                List<bool> districtCodeBool = districtCodeString
                    .Substring(1, districtCodeString.Count() - 2) // throw bracets away
                    .Split(',')
                    .Cast<bool>()
                    .ToList();
                namesCodes.Add(districtName, districtCodeBool);
                line = sr.ReadLine();
            }
        }

        public static List<bool> getCode(string name)
        {
            return namesCodes[name];
        }
    }

    public record TrainData(List<Collage> Collages, List<District> Districts);

    public record Collage(Location Location, string Name);

    public record District(Location Location, List<bool> Code);

    public class Location
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
