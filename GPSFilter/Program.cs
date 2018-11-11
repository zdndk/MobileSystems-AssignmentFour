using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedCsv = ParseCSV();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            ApplyMedianFilter(parsedCsv.Count, parsedCsv.LatList, parsedCsv.LongList, 5);
            stopwatch.Stop();


            SaveCsv(parsedCsv, @"C:\Users\d-loa\Desktop\Leaflet Exercise\BikingPhoneMedianFilter.csv");

            parsedCsv = ParseCSV();


            stopwatch.Reset();
            stopwatch.Start();
            ApplyMeanFilter(parsedCsv.Count, parsedCsv.LatList, parsedCsv.LongList, 5);
            stopwatch.Stop();


            SaveCsv(parsedCsv, @"C:\Users\d-loa\Desktop\Leaflet Exercise\BikingPhoneMeanFilter.csv");

        }

        private static void SaveCsv(LatLongSet latLongSet, string path)
        {
            using (var streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine("latitude,longitude");
                for (int i = 0; i < latLongSet.Count; i++)
                {
                    streamWriter.WriteLine(latLongSet.LatList[i] + "," + latLongSet.LongList[i]);
                }
            }
        }

        private static LatLongSet ParseCSV()
        {
            var latList = new List<double>();
            var longList = new List<double>();
 
            using (var streamReader = new StreamReader(@"C:\Users\d-loa\Desktop\Leaflet Exercise\BikingPhoneNoFilter.csv"))
            {
                streamReader.ReadLine(); //skip headers
                string readLine = string.Empty;

                while ((readLine = streamReader.ReadLine()) != null)
                {
                    var csvSplit = readLine.Split(',');
                    latList.Add(double.Parse(csvSplit[0]));
                    longList.Add(double.Parse(csvSplit[1]));
                }
            }

            if (latList.Count != longList.Count)
            {
                throw new Exception("Non-matching coordinate count.");
            }
            
            return new LatLongSet() {LatList = latList, LongList = longList};
        }

        private static void ApplyMedianFilter(int count, IList<double> latList, IList<double> longList, int boxsize)
        {
            for (var i = 0; i < count; i++)
            {
                var lookAheadDistance = boxsize + i > count ? boxsize + (count - (boxsize + i)) : boxsize; //boxsize will approach 0 as we move to the end

                if (lookAheadDistance == 1) //edgecase for the last element
                {
                    return; //don't change the value, return since there is nothing more to change
                }

                var latSubset = latList.Skip(i).Take(lookAheadDistance).ToList();
                var longSubset = longList.Skip(i).Take(lookAheadDistance).ToList();

                latSubset.Sort();
                longSubset.Sort();
                var middleIndex = ((lookAheadDistance + 1) / 2) - 1;

                if (lookAheadDistance % 2 == 0) //even numbers
                {
                    //for even numbers, take the average of the middle values in the subset
                    latList[i] = (latSubset[middleIndex] + latSubset[middleIndex + 1]) / 2;
                    longList[i] = (longSubset[middleIndex] + longSubset[middleIndex + 1]) / 2;
                }
                else //uneven numbers
                {
                    //Take middle value in subset, subtract 1 to adjust for 0 based indexing
                    latList[i] = latSubset[middleIndex];
                    longList[i] = longSubset[middleIndex];
                }
            }
        }

        private static void ApplyMeanFilter(int count, IList<double> latList, IList<double> longList, int boxsize)
        {
            for (var i = 0; i < count; i++)
            {
                var lookAheadDistance = boxsize + i > count ? boxsize + (count - (boxsize + i)) : boxsize; //boxsize will approach 0 as we move to the end

                if (lookAheadDistance == 1) //edgecase for the last element
                {
                    return; //don't change the value, return since there is nothing more to change
                }

                var latSubset = latList.Skip(i).Take(lookAheadDistance).ToList();
                var longSubset = longList.Skip(i).Take(lookAheadDistance).ToList();

                latList[i] = latSubset.Average();
                longList[i] = longSubset.Average();
            }
        }
    }
}
