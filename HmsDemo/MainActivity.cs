using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Huawei.Hms.Api;
using Huawei.Hms.Location;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Clustering;
using Huawei.Hms.Maps.Model;

namespace HmsDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "MapViewDemoActivity";

        private HuaweiMap _map;
        private MapView _mapView;

        private const string MapViewBundleKey = "MapViewBundleKey";
        private LatLng _latLng;
        private Circle _circle;
        private Marker _marker;
        private FusedLocationProviderClient _fusedLocationProviderClient;
        private LastLocationListener _fusedLocationProviderClientLastLocationListener;
        private DelegateLocationCallback _locationCallback;
        private ClusterManager _clusterManager;
        private static readonly string[] Permissions = new string[]
        {
            Android.Manifest.Permission.AccessCoarseLocation,
            Android.Manifest.Permission.AccessFineLocation,
            Android.Manifest.Permission.Internet
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (HuaweiApiAvailability.Instance.IsHuaweiMobileServicesAvailable(this) != ConnectionResult.Success)
                return;

            // Initialise AGConnectServices here or in XamarinCustomProvider
            // var config = AGConnectServicesConfig.FromContext(ApplicationContext);
            // config.OverlayWith(new HmsLazyInputStream(this));
            // AGConnectInstance.Initialize(this);

            // Initialise huawei fused location provider
            _fusedLocationProviderClientLastLocationListener = new LastLocationListener(
                location =>
                {
                    switch (location)
                    {
                        case null:
                            GetLocation();
                            break;
                        default:
                            MoveToUserLocation(new LatLng(location.Latitude, location.Longitude));
                            break;
                    }
                },
                exception =>
                {
                    Toast.MakeText(this, exception.Message, ToastLength.Long).Show();
                });
            _fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);

            // Get last known location if permissions are granted
            if (CheckPermission(Permissions, 100))
                GetLastLocation();

            // Initialise huawei map
            _mapView = FindViewById<MapView>(Resource.Id.mapview);
            Bundle mapViewBundle = null;
            if (savedInstanceState != null)
                mapViewBundle = savedInstanceState.GetBundle(MapViewBundleKey);
            _mapView.OnCreate(mapViewBundle);
            RunOnUiThread(() => _mapView.GetMapAsync(this));
        }

        private void GetLastLocation()
        {
            _fusedLocationProviderClient.GetLastLocation()
                .AddOnSuccessListener(_fusedLocationProviderClientLastLocationListener)
                .AddOnFailureListener(_fusedLocationProviderClientLastLocationListener);
        }
  
        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "onMapReady: ");
            _map = map;

            _map.UiSettings.ZoomControlsEnabled = true;
            _map.UiSettings.CompassEnabled = true;
            _map.UiSettings.MyLocationButtonEnabled = true;

            _map.MyLocationEnabled = true;
            _map.MapType = HuaweiMap.MapTypeNormal;

            Toast.MakeText(this, "OnMapReady done.", ToastLength.Short).Show();

            _clusterManager = new ClusterManager(this, _map);
            _map.SetOnCameraIdleListener(_clusterManager);

            _clusterManager.SetCallbacks(new ClusterManagerCallbacks());

            AddRandomClusterItems(_clusterManager);
            _clusterManager.SetIconGenerator(new CustomIconGenerator(this));
        }

        private void AddRandomClusterItems(ClusterManager clusterManager)
        {
            LatLngBounds germany = new LatLngBounds(new LatLng(47.77083, 6.57361), new LatLng(53.35917, 12.10833));

            int buffer = 10000;
            var clusterItems = new List<SampleClusterItem>(buffer);
            for (int i = 0; i < buffer; i++)
            {
                clusterItems.Add(RandomLocationGenerator.Generate(germany));
            }
            clusterManager.AddItems(clusterItems);
            clusterItems.Clear();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _mapView.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _mapView.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _mapView.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _mapView.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _mapView.OnResume();
        }

        public void MoveToUserLocation(LatLng location)
        {
            if (location == null || _map == null)
                return;
            _latLng = location;
            CameraPosition build = new CameraPosition.Builder().Target(_latLng).Zoom(14).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(build);
            _map.AnimateCamera(cameraUpdate);
            AddCircle();
        }

        private void AddCircle()
        {
            if (_map == null || _latLng == null)
            {
                return;   
            }

            _circle = _map.AddCircle(new CircleOptions()
                .Center(_latLng)
                .Radius(1000).
                FillColor(new Android.Graphics.Color(0x53, 0x43, 0x51, 0x33)));
            _marker = _map.AddMarker(new MarkerOptions()
                .Position(_latLng)
                .Icon(BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_map_marker))
                .Clusterable(false)
            );

            _marker.ShowInfoWindow();
        }

        public void GetLocation()
        {
            LocationRequest locationRequest = new LocationRequest();
            locationRequest.SetInterval(1000).SetPriority(LocationRequest.PriorityHighAccuracy);

            _locationCallback = new DelegateLocationCallback(locationResult =>
            {
                if (locationResult != null && locationResult.HWLocationList.Count > 0)
                {
                    var location = locationResult.HWLocationList[0];
                    MoveToUserLocation(new LatLng(location.Latitude, location.Longitude));
                    _fusedLocationProviderClient.RemoveLocationUpdates(_locationCallback);
                }
            });

            _fusedLocationProviderClient.RequestLocationUpdates(locationRequest, _locationCallback, MainLooper);

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
                GetLastLocation();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _fusedLocationProviderClientLastLocationListener?.Dispose();
            _fusedLocationProviderClientLastLocationListener = null;

            _fusedLocationProviderClient?.Dispose();
            _fusedLocationProviderClient = null;

            _circle?.Dispose();
            _circle = null;

            _latLng?.Dispose();
            _latLng = null;

            _marker?.Dispose();
            _marker = null;
        }
    }
}
