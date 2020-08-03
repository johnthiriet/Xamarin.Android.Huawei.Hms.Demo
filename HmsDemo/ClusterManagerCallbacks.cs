using Android.Util;
using Huawei.Hms.Maps.Clustering;

namespace HmsDemo
{
    public class ClusterManagerCallbacks : Java.Lang.Object, ClusterManager.ICallbacks
    {
        private const string Tag = "ClusterManagerCallbacks";

        public bool OnClusterClick(Cluster cluster)
        {
            Log.Debug(Tag, "OnClusterClick");
            return false;
        }

        public bool OnClusterItemClick(Java.Lang.Object clusterItem)
        {
            Log.Debug(Tag, "OnClusterItemClick");
            return false;
        }
    }    
}
