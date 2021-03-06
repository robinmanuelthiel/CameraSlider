﻿using Android.OS;
using CameraSlider.Frontend.Forms.Droid.Services;
using CameraSlider.Frontend.Forms.Services;

[assembly: Xamarin.Forms.Dependency(typeof(EnvironmentCheckServiceAndroid))]
namespace CameraSlider.Frontend.Forms.Droid.Services
{
    public class EnvironmentCheckServiceAndroid : IEnvironmentCheckService
    {
        public bool IsSimulatorOrEmulator()
        {
            if (Build.Fingerprint != null)
            {
                if (Build.Fingerprint.Contains("vbox") || Build.Fingerprint.Contains("generic"))
                    return true;
            }

            return false;
        }
    }
}
