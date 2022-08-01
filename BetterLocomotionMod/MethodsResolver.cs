using System.Reflection;
using System.Linq;
using MelonLoader;

/*
 * Code by SDraw
 */

namespace BetterLocomotion
{
    internal static class MethodsResolver
    {
        internal static MelonLogger.Instance Logger;

        private static MethodInfo ms_playerLocomotion;
        private static MethodInfo ms_prepareForCalibration;
        private static MethodInfo ms_restoreTrackingAfterCalibration;

        public static void ResolveMethods(MelonLogger.Instance loggerInstance)
        {
            Logger = loggerInstance;

            if (ms_playerLocomotion == null)
            {
                var l_methods = ;// TODO get method
                );

                if (l_methods.Any())
                {
                    ms_prepareForCalibration = l_methods.First();
                }
                else
                {
                    Logger.Error("Can't resolve player's locomotion method");
                }
            }

            // void VRCTrackingManager.PrepareForCalibration()
            if (ms_prepareForCalibration == null)
            {
                var l_methods = ;// TODO get method
                );

                if (l_methods.Any())
                {
                    ms_prepareForCalibration = l_methods.First();
                }
                else
                    Logger.Warning("Can't resolve start calibration's method");
            }

            if (ms_restoreTrackingAfterCalibration == null)
            {
                var l_methods = ;// TODO get method
                );

                if (l_methods.Any())
                {
                    ms_restoreTrackingAfterCalibration = l_methods.First();
                }
                else
                {
                    Logger.Warning("Can't resolve finish calibration's method");
                }
            }
        }

        public static MethodInfo PlayerLocomotion
        {
            get => ms_restoreTrackingAfterCalibration;
        }
        public static MethodInfo PrepareForCalibration
        {
            get => ms_prepareForCalibration;
        }
        public static MethodInfo RestoreTrackingAfterCalibration
        {
            get => ms_restoreTrackingAfterCalibration;
        }
    }
}