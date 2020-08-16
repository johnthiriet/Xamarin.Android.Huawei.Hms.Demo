using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.HmsScanKit;
using Huawei.Hms.Ml.Scan;

namespace HmsDemo
{
    [Activity]
    public class ScanActivity : AppCompatActivity, IOnResultCallback
    {
        public const string TAG = nameof(ScanActivity);
        public const string SCAN_RESULT = "scanResult";

        private RemoteView remoteView;
        int mScreenWidth;
        int mScreenHeight;
        int SCAN_FRAME_SIZE = 300;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_scan);

            DisplayMetrics dm = Resources.DisplayMetrics;
            float density = dm.Density;

            mScreenWidth = dm.WidthPixels;
            mScreenHeight = dm.HeightPixels;

            int scanFrameSize = (int)(SCAN_FRAME_SIZE * density);

            Rect rect = new Rect();
            rect.Left = mScreenWidth / 2 - scanFrameSize / 2;
            rect.Right = mScreenWidth / 2 + scanFrameSize / 2;
            rect.Top = mScreenHeight / 2 - scanFrameSize / 2;
            rect.Bottom = mScreenHeight / 2 + scanFrameSize / 2;

            remoteView = new RemoteView.Builder()
                .SetContext(this)
                .SetBoundingBox(rect)
                .SetFormat(HmsScan.AllScanType)
                .Build();

            remoteView.OnCreate(savedInstanceState);
            remoteView.SetOnResultCallback(this);

            FrameLayout.LayoutParams @params = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent, FrameLayout.LayoutParams.MatchParent);
            FrameLayout frameLayout = FindViewById<FrameLayout>(Resource.Id.rim);
            frameLayout.AddView(remoteView, @params);

            //set back button listener
            ImageView backBtn = FindViewById<ImageView>(Resource.Id.back_img);
            backBtn.Click += (s, a) => Finish();
        }

        public void OnResult(HmsScan[] result)
        {
            if (result != null && result.Length > 0 && result[0] != null && !TextUtils.IsEmpty(result[0].OriginalValue))
            {
                Intent intent = new Intent();
                intent.PutExtra(SCAN_RESULT, result[0]);
                SetResult(Result.Ok, intent);
                Finish();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            remoteView.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();
            remoteView.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            remoteView.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            remoteView.OnDestroy();
        }

        protected override void OnStop()
        {
            base.OnStop();
            remoteView.OnStop();
        }
    }
}
