using System;
using Android.Content;
using Android.Runtime;
using Huawei.Hms.Maps.Clustering;
using Huawei.Hms.Maps.Model;
using Huawei.Hms.Maps.Util;

namespace HmsDemo
{
    public class CustomIconGenerator : DefaultIconGenerator
    {
        public CustomIconGenerator(Context context) : base(context)
        {
        }

        protected CustomIconGenerator(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override BitmapDescriptor GetClusterItemIcon(Java.Lang.Object clusterItem)
        {
            return BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_map_marker);
        }

    }
}
