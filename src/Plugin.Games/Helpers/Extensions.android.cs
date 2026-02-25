
using System.Threading.Tasks;
using Android.Gms.Tasks;
using Java.Interop;

namespace Plugin.Games
{
    internal static class Extensions
    {
        public static Task<T> AsTask<T>(this Android.Gms.Tasks.Task task) where T : Java.Lang.Object
        {
            var tcs = new TaskCompletionSource<T>();

            task.AddOnCompleteListener(new OnCompleteListener<T>(tcs));

            return tcs.Task;
        }

        private sealed class OnCompleteListener<T> : Java.Lang.Object, IOnCompleteListener where T : Java.Lang.Object
        {
            private readonly TaskCompletionSource<T> _tcs;

            public OnCompleteListener(TaskCompletionSource<T> tcs)
            {
                _tcs = tcs;
            }

            public void OnComplete(Android.Gms.Tasks.Task task)
            {
                if (task.IsCanceled)
                {
                    _tcs.TrySetCanceled();
                }
                else if (!task.IsSuccessful)
                {
                    _tcs.TrySetException(task.Exception);
                }
                else
                {
                    var result = task.Result.JavaCast<T>();
                    _tcs.TrySetResult(result);
                }
            }
        }

        /// <summary>
        /// Helper to convert Android.Net.Uri to System.Uri
        /// </summary>
        internal static System.Uri ToUri(this Android.Net.Uri androidUri)
        {
            if (androidUri == null)
                return null;

            return new System.Uri(androidUri.ToString());
        }
    }
}
