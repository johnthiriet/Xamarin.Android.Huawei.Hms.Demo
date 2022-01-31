using System;
using Android.Util;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Support.Api.Entity.SafetyDetect;

namespace HmsDemo
{
    public partial class MainActivity
    {
        public class SafetyDetectListener :
        Java.Lang.Object,
        IOnSuccessListener,
        IOnFailureListener,
        IOnCanceledListener,
        IOnCompleteListener
        {
            private const string TAG = "LastLocationListener";

            private Action<SysIntegrityResp> _onSuccess;
            private Action<Java.Lang.Exception> _onFailure;

            public SafetyDetectListener(
                Action<SysIntegrityResp> onSuccess,
                Action<Java.Lang.Exception> onFailure)
            {
                _onSuccess = onSuccess;
                _onFailure = onFailure;
            }

            public void OnFailure(Java.Lang.Exception exception)
            {
                Log.Info(TAG, "On failure received with {0}", exception?.ToString() ?? "null");

                _onFailure?.Invoke(exception);
            }

            public void OnSuccess(Java.Lang.Object parameter)
            {
                SysIntegrityResp response = parameter as SysIntegrityResp;

                Log.Info(TAG, "On success received with {0}", response?.ToString() ?? "null");

                _onSuccess?.Invoke(response);
            }

            public void OnCanceled()
            {
                Log.Info(TAG, "On canceled received");
            }

            public void OnComplete(Task task)
            {
                Log.Info(TAG, "On completed received");
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                _onSuccess = null;
                _onFailure = null;
            }
        }
    }
}
