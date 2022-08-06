using System.Reflection;
using System.Linq;
using MelonLoader;

namespace BetterLocomotion
{
    internal static class MethodsResolver
    {
        internal static MelonLogger.Instance Logger;

        private static MethodInfo ms_playerLocomotion;
        private static MethodInfo ms_startCalibration;
        private static MethodInfo ms_finishCalibration;

        public static void ResolveMethods(MelonLogger.Instance loggerInstance)
        {
            Logger = loggerInstance;

            if (ms_playerLocomotion == null)
            {
                var l_methods = nu; // TODO get method
                );

                if (l_methods.Any())
                {
                    ms_startCalibration = l_methods.First();
                }
                else
                {
                    Logger.Error("Can't resolve player's locomotion method");
                }
            }

            // void VRCTrackingManager.PrepareForCalibration()
            if (ms_startCalibration == null)
            {
                var l_methods = ; // TODO get method
                );

                if (l_methods.Any())
                {
                    ms_startCalibration = l_methods.First();
                }
                else
                    Logger.Warning("Can't resolve start calibration's method");
            }

            if (ms_finishCalibration == null)
            {
                var l_methods = ; // TODO get method
                );

                if (l_methods.Any())
                {
                    ms_finishCalibration = l_methods.First();
                }
                else
                {
                    Logger.Warning("Can't resolve finish calibration's method");
                }
            }
        }

        public static MethodInfo PlayerLocomotion
        {
            get => ms_finishCalibration;
        }
        public static MethodInfo StartCalibration
        {
            get => ms_startCalibration;
        }
        public static MethodInfo FinishCalibration
        {
            get => ms_finishCalibration;
        }
    }
}