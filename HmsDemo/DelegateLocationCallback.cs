using System;
using Huawei.Hms.Location;

namespace HmsDemo
{
    public class DelegateLocationCallback : LocationCallback
    {
        private Action<LocationResult> _onLocationResult;

        public DelegateLocationCallback(Action<LocationResult> onLocationResult)
        {
            _onLocationResult = onLocationResult;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _onLocationResult = null;
        }

        public override void OnLocationResult(LocationResult locationResult)
        {
            _onLocationResult?.Invoke(locationResult);
        }
    }
}
