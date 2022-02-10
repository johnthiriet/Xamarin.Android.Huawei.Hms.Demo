using System;
using Android.Content;
using Android.Database;
using Android.Util;
using Huawei.Agconnect.Config;

namespace HmsDemo
{
    [ContentProvider(new string[] { "com.johnthiriet.XamarinSample" }, InitOrder = 999)]
    public class XamarinCustomProvider : ContentProvider
    {
        public override int Delete(Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }

        public override string GetType(Android.Net.Uri uri)
        {
            throw new NotImplementedException();
        }

        public override Android.Net.Uri Insert(Android.Net.Uri uri, ContentValues values)
        {
            throw new NotImplementedException();
        }

        public override bool OnCreate()
        {
            Log.Info("XamarinCustomProvider", "OnCreate : Start");

            var config = AGConnectServicesConfig.FromContext(Context);
            config.OverlayWith(new HmsLazyInputStream(Context));

            Log.Info("XamarinCustomProvider", "OnCreate : End");
            return false;
        }

        public override ICursor Query(Android.Net.Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            throw new NotImplementedException();
        }

        public override int Update(Android.Net.Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }
    }
}