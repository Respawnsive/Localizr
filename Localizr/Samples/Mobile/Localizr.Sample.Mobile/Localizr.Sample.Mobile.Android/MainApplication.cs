using System;
using Android.App;
using Android.Runtime;
using Shiny;

namespace Localizr.Sample.Mobile.Droid
{
    [Application]
    public class MainApplication : ShinyAndroidApplication<Startup>
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }
    }
}