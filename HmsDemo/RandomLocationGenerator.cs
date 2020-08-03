using System;
using Huawei.Hms.Maps.Model;

namespace HmsDemo
{
    public class RandomLocationGenerator
    {
        private static Random RANDOM = new Random();

        public static SampleClusterItem Generate(LatLngBounds bounds)
        {
            double minLatitude = bounds.Southwest.Latitude;
            double maxLatitude = bounds.Northeast.Latitude;
            double minLongitude = bounds.Southwest.Longitude;
            double maxLongitude = bounds.Northeast.Longitude;

            var latitude = minLatitude + (maxLatitude - minLatitude) * RANDOM.NextDouble();
            var longitude = minLongitude + (maxLongitude - minLongitude) * RANDOM.NextDouble();

            return new SampleClusterItem(latitude, longitude);
        }
    }
}
