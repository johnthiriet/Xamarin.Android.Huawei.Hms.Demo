using System;
using System.IO;
using Android.Content;
using Huawei.Agconnect.Config;

namespace HmsDemo
{
    class HmsLazyInputStream : LazyInputStream
    {
        public HmsLazyInputStream(Context context)
         : base(context)
        {
        }

        public override Stream Get(Context context)
        {
            try
            {
                return context.Assets.Open("agconnect-services.json");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}