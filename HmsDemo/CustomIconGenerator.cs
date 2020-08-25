using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Huawei.Hms.Maps.Clustering;

namespace HmsDemo
{
    public class CustomIconGenerator : DefaultIconGenerator
    {
        private Context _context;

        public CustomIconGenerator(Context context) : base(context)
        {
            _context = context;
            SetIconStyle(CreateDefaultIconStyle());
        }

        protected CustomIconGenerator(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private IconStyle CreateDefaultIconStyle()
        {
            return new IconStyle.Builder(_context)
                .SetClusterBackgroundColor(new Color(0x6c, 0xa6, 0xc1))
                .Build();
        }
    }
}
