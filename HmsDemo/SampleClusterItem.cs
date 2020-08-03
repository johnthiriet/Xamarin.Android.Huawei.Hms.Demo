using Huawei.Hms.Maps.Clustering;

namespace HmsDemo
{
    public class SampleClusterItem : Java.Lang.Object, IClusterItem
    {
        public SampleClusterItem(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }

        public double Longitude { get; }

        public string Snippet => null;

        public string Title => null;
    }
}
