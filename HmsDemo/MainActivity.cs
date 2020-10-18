using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Huawei.Agconnect;
using Huawei.Agconnect.Config;
using Huawei.Hms.Analytics;
using Huawei.Hms.HmsScanKit;
using Huawei.Hms.Ml.Scan;
using Huawei.Hms.Push;

namespace HmsDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        private const string TAG = nameof(MainActivity);
        public const int DEFINED_CODE = 222;
        public const int REQUEST_CUSTOM_CODE_SCAN = 0x01;
        public const int REQUEST_CLASSIC_CODE_SCAN = 0x02;

        private Button _custom_scan_btn;
        private Button _classic_scan_btn;
        private Button _push_btn;
        private Button _map_btn;

        private HiAnalyticsInstance _hiAnalyticsInstance;

        private void SendEvent(string key, string value)
        {
            // Initiate Parameters            
            Bundle bundle = new Bundle();
            bundle.PutString(key, value);
            // Report a custom Event            
            _hiAnalyticsInstance?.OnEvent(key, bundle);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            // Initialise AGConnectServices here or in XamarinCustomProvider
            var config = AGConnectServicesConfig.FromContext(ApplicationContext);
            config.OverlayWith(new HmsLazyInputStream(this));
            AGConnectInstance.Initialize(this);

            HiAnalyticsTools.EnableLog();
            _hiAnalyticsInstance = HiAnalytics.GetInstance(this);
            _hiAnalyticsInstance.SetAnalyticsEnabled(true);


            _custom_scan_btn = FindViewById<Button>(Resource.Id.custom_scan_btn);
            _classic_scan_btn = FindViewById<Button>(Resource.Id.classic_scan_btn);
            _push_btn = FindViewById<Button>(Resource.Id.push_btn);
            _map_btn = FindViewById<Button>(Resource.Id.map_btn);

            _map_btn.Click += OnMapButtonClicked;
            _push_btn.Click += OnPushButtonClicked;
            _custom_scan_btn.Click += OnCustomScanButtonClicked;
            _classic_scan_btn.Click += OnClassicScanButtonClicked;
        }

        private void OnMapButtonClicked(object sender, EventArgs e)
        {
            SendEvent("Tap", "Map");
            StartActivity(new Intent(this, typeof(MapActivity)));
        }

        private void OnPushButtonClicked(object sender, EventArgs e)
        {
            //var instance = HmsMessaging.GetInstance(ApplicationContext);
            //instance.AutoInitEnabled = true;
            SendEvent("Tap", "Push");
            var token = Xamarin.Essentials.Preferences.Get("PushToken", string.Empty);

            ClipboardManager clipboard = (ClipboardManager)GetSystemService(Context.ClipboardService);
            ClipData clip = ClipData.NewPlainText("Push Token", token);
            clipboard.PrimaryClip = clip;

            Log.Info(TAG, $"Push token: {token}");
            Toast.MakeText(this, $"Push token: {token} added to clipboard", ToastLength.Short).Show();
        }

        private void OnClassicScanButtonClicked(object sender, EventArgs e)
        {
            SendEvent("Tap", "ClassicScan");
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (CheckPermission(new string[] { Android.Manifest.Permission.Camera }, DEFINED_CODE))
                {
                    ScanUtil.StartScan(this, REQUEST_CLASSIC_CODE_SCAN, new HmsScanAnalyzerOptions.Creator().SetHmsScanTypes(HmsScan.AllScanType).Create());
                }
            }
        }

        private void OnCustomScanButtonClicked(object sender, EventArgs e)
        {
            SendEvent("Tap", "CustomScan");

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (CheckPermission(new string[] { Android.Manifest.Permission.Camera}, DEFINED_CODE))
                {
                    StartActivityForResult(new Intent(this, typeof(ScanActivity)), REQUEST_CUSTOM_CODE_SCAN);
                }
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok || data == null)
            {
                return;
            }

            HmsScan hmsScan;

            switch (requestCode)
            {
                case REQUEST_CUSTOM_CODE_SCAN:
                    hmsScan = data.GetParcelableExtra(ScanActivity.SCAN_RESULT) as HmsScan;
                    break;
                case REQUEST_CLASSIC_CODE_SCAN:
                    hmsScan = data.GetParcelableExtra(ScanUtil.Result) as HmsScan;
                    break;
                default:
                    hmsScan = null;
                    break;
            }

            if (hmsScan != null && !string.IsNullOrWhiteSpace(hmsScan.OriginalValue))
            {
                Toast.MakeText(this, hmsScan.OriginalValue, ToastLength.Short).Show();
            }
        }

        #region Permissions
        public bool CheckPermission(string[] permissions, int requestCode)
        {
            var hasAllPermissions = true;
            foreach (string permission in permissions)
            {
                if (ContextCompat.CheckSelfPermission(this, permission) == Permission.Denied)
                {
                    hasAllPermissions = false;
                    ActivityCompat.RequestPermissions(this, permissions, requestCode);
                }
            }

            return hasAllPermissions;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            bool hasAllPermissions = true;
            for (int i = 0; i < permissions.Length; i++)
            {
                if (grantResults[i] == Permission.Denied)
                {
                    hasAllPermissions = false;
                    break;
                }
            }


            if (hasAllPermissions)
            {
                if (requestCode == DEFINED_CODE)
                {
                    StartActivityForResult(new Intent(this, typeof(ScanActivity)), REQUEST_CUSTOM_CODE_SCAN);
                }
            }
        }
        #endregion
    }
}
