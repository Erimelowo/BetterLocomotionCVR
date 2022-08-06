using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.IO;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;
using MelonLoader;
using DecaSDK;
using ABI_RC.Core;
using ABI_RC.Core.Player;

[assembly: AssemblyCopyright("Created by " + "Erimel & AxisAngle")]
[assembly: MelonInfo(typeof(BetterLocomotion.Main), "BetterLocomotion", "0.1.0", "Erimel & AxisAngle")]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonColor(ConsoleColor.Magenta)]
[assembly: MelonOptionalDependencies("TODO")]

namespace BetterLocomotion
{
    public class Main : MelonMod
    {
        private enum Locomotion { Head, Hip, /*Chest,*/ Deca }

        internal static MelonLogger.Instance Logger;
        private static HarmonyLib.Harmony _hInstance;

        private static bool _xrPresent;

        private static DecaMoveBehaviour deca;

        public override void OnApplicationStart()
        {
            Logger = LoggerInstance;
            _hInstance = HarmonyInstance;

            InitializeSettings();
            OnPreferencesSaved();

            MelonCoroutines.Start(WaitForXRDevice());
        }

        IEnumerator WaitForXRDevice()
        {
            // Waits for XRDevice.isPresent to be defined
            yield return new WaitForSeconds(0.1f);

            _xrPresent = XRDevice.isPresent;
            if (_xrPresent)
            {
                InitializeMod();
            }
            else
            {
                Logger.Warning("Did not initialize; Mod is VR-Only.");
            }
        }

        private void InitializeMod()
        {
            // Patches
            MethodsResolver.ResolveMethods(Logger);
            if (MethodsResolver.PlayerLocomotion != null)
                HarmonyInstance.Patch(MethodsResolver.PlayerLocomotion, null,
                    new HarmonyMethod(typeof(Main), nameof(CalculateDirection)));
            if (MethodsResolver.StartCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.StartCalibration, null,
                    new HarmonyMethod(typeof(Main), nameof(StartFBTCalibration)));
            if (MethodsResolver.FinishCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.FinishCalibration, null,
                    new HarmonyMethod(typeof(Main), nameof(FinishFBTCalibration)));

            Logger.Msg("Successfully initialized!");
        }

        private static MelonPreferences_Entry<Locomotion> _locomotionMode;
        private static void InitializeSettings()
        {
            MelonPreferences.CreateCategory("BetterLocomotion", "BetterLocomotion");

            _locomotionMode = MelonPreferences.CreateEntry("BetterLocomotion", "LocomotionMode", Locomotion.Head, "Locomotion mode");
        }

        public override void OnApplicationQuit()
        {
            if (deca != null) deca.OnDestroy();
        }

        private static bool _decaDllLoaded;
        public override void OnPreferencesSaved()
        {
            if (_locomotionMode.Value == Locomotion.Deca)
            {
                if (!_decaDllLoaded)
                {
                    string dllName = "deca_sdk.dll";

                    try
                    {
                        using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Main), dllName);
                        using var fileStream = File.Open("ChilloutVR_Data/Plugins/" + dllName, FileMode.Create, FileAccess.Write);
                        resourceStream.CopyTo(fileStream);
                    }
                    catch (IOException ex)
                    {
                        MelonLogger.Warning("Failed to write native dll; will attempt loading it anyway. This is normal if you're running multiple instances of CVR");
                        MelonDebug.Msg(ex.ToString());
                    }
                    _decaDllLoaded = true;
                }

                if (deca == null)
                {
                    deca = new DecaMoveBehaviour
                    {
                        Logger = Logger

                    };
                    Logger.Msg("Deca Created");
                }

                //if (decaButton != null) decaButton.enabled = true;
            }
            else
            {
                //if (decaButton != null) decaButton.enabled = false;
            }
        }


        private static PlayerSetup GetLocalPlayer() => PlayerSetup.Instance;

        private static bool CheckIfInFbt() => GetLocalPlayer().fullBodyActive;

        private static VRTrackerManager GetTrackerManager() => GetLocalPlayer()._trackerManager;

        public override void OnUpdate()
        {
            if (_xrPresent)
            {
                if (_locomotionMode.Value == Locomotion.Deca && deca != null)
                {
                    deca.Update();
                }
            }
        }

        private static void StartFBTCalibration()
        {
            _isCalibrating = true;
        }

        private static void FinishFBTCalibration()
        {
            _isCalibrating = false;
            _isInFbt = true;

            if (_offsetHip == null)
            {
                _hipTransform = GetTrackerManager().hipCandidate.transform;
                //_chestTransform = GetTrackerManager().chestCandidate.transform;

                Quaternion rotation = Quaternion.FromToRotation(_headTransform.up, Vector3.up) * _headTransform.rotation;

                _offsetHip = new GameObject
                {
                    transform =
                {
                    parent = _hipTransform,
                    rotation = rotation
                }
                };
                /*_offsetChest = new GameObject
                {
                    transform =
                {
                    parent = _chestTransform,
                    rotation = rotation
                }
                };*/
            }
        }

        private static bool _isInFbt, _isCalibrating;
        private static float _avatarScaledSpeed = 1;
        private static GameObject _offsetHip, _offsetChest;
        private static Transform _headTransform, _hipTransform, _chestTransform;
        private static InputManager inputManager;
        private static Transform HeadTransform => // Gets the head transform
            _headTransform ??= GetLocalPlayer().vrHeadTracker.transform;

        // Fixes the game's original direction to match the preferred one
        private static Vector3 CalculateDirection()
        {
            if (_locomotionMode.Value == Locomotion.Deca && deca != null) deca.HeadTransform = HeadTransform;

            _isInFbt = CheckIfInFbt();

            Vector3 @return = _locomotionMode.Value switch
            {
                Locomotion.Hip when _isInFbt && !_isCalibrating && _hipTransform != null => CalculateLocomotion(_offsetHip.transform),
                // Locomotion.Chest when _isInFbt && !_isCalibrating && _chestTransform != null => CalculateLocomotion(_offsetChest.transform),
                Locomotion.Deca when deca != null && (deca.state == Move.State.Streaming) && deca.OutObject => CalculateLocomotion(deca.OutTransform),
                _ => CalculateLocomotion(HeadTransform),
            };

            return @return;
        }

        private static Vector3 CalculateLocomotion(Transform trackerTransform) // Thanks AxisAngle for the math!
        {
            // Get joystick inputs
            float inputX = ;
            float inputY = ;
            float inputMag = Mathf.Sqrt(inputX * inputX + inputY * inputY);

            // Early escape to avoid division by 0
            if (inputMag == 0.0f) return Vector3.zero;

            // Now we modulate the input magnitude to observe a deadzone
            float in0 = Mathf.Clamp(_joystickThreshold.Value, 0, 0.96f), in1 = 1.0f;
            float out0 = 0.0f, out1 = 1.0f;
            float inputMod = Mathf.Clamp(LinearMap(in0, in1, out0, out1, inputMag), out0, out1);

            // Check if inputMod is 0 to avoid weird bugs...
            if (inputMod == 0) return Vector3.zero;

            // Get player speeds
            float speedMod;
            VRCMotionState playerMotionState = GetLocalPlayer().gameObject.GetComponent<VRCMotionState>();
            if (playerMotionState.field_Private_Single_0 < 0.4f) speedMod = 0.1f;
            else if (playerMotionState.field_Private_Single_0 < 0.65f) speedMod = 0.5f;
            else speedMod = 1.0f;
            if (_lolimotion.Value == true) speedMod *= _avatarScaledSpeed;

            VRCPlayerApi playerApi = ;
            float ovalWidth = inputMod * speedMod * playerApi.GetStrafeSpeed();
            float ovalHeight = inputMod * speedMod * playerApi.GetRunSpeed();

            // Compute the multiplier which moves the input onto the oval
            float t = TimeToOval(ovalWidth, ovalHeight, inputX, inputY);

            // Apply t to get a point on the oval and compute input direction with given inputs
            Vector3 inputDirection = t * (inputX * Vector3.right + inputY * Vector3.forward);
            return Quaternion.FromToRotation(trackerTransform.transform.up, Vector3.up) * trackerTransform.transform.rotation * inputDirection;
        }

        // Linear mapping
        private static float LinearMap(float x0, float x1, float y0, float y1, float x)
        {
            return ((x1 - x) * y0 + (x - x0) * y1) / (x1 - x0);
        }

        // Raycast from the center against an oval
        private static float TimeToOval(float w, float h, float dx, float dy)
        {
            // compute time of intersection time between ray d and the oval
            return 1.0f / Mathf.Sqrt(dx * dx / (w * w) + dy * dy / (h * h));
        }
    }
}
