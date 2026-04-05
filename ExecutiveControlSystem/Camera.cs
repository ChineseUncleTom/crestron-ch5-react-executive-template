using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;

namespace ExecutiveControlSystem
{
    /// <summary>
    /// Encapsulates all camera control logic for up to ten cameras.
    ///
    /// <para>
    /// Each camera supports:
    /// <list type="bullet">
    ///   <item>Pan / Tilt / Zoom (hold joins 90–95 – both press and release edges)</item>
    ///   <item>Power toggle with feedback (button join 96 / feedback join 118)</item>
    ///   <item>Auto-tracking toggle with feedback (button join 97 / feedback join 119)</item>
    ///   <item>Preset recall (joins 98–107)</item>
    ///   <item>Camera select with active feedback (button joins 108–117 / feedback joins 120–129)</item>
    ///   <item>Zoom speed analog set / feedback (set join 15 / feedback join 16)</item>
    ///   <item>Moment speed analog set / feedback (set join 17 / feedback join 18)</item>
    ///   <item>Camera names sent to panel on startup (serial joins 34–43)</item>
    ///   <item>Preset names for selected camera sent to panel (serial joins 44–53)</item>
    /// </list>
    /// </para>
    /// </summary>
    internal class Camera
    {
        private const int MAX_CAMERAS = 10;
        private const int MAX_PRESETS = 10;

        private readonly BasicTriListWithSmartObject _panel;
        private readonly SystemInfo _systemInfo;

        private bool   _powerOn;
        private bool   _trackingOn;
        private int    _selectedCamera = 1; // 1-based; defaults to 1 (first camera)
        private ushort _zoomSpeed;
        private ushort _momentSpeed;

        // ── Join arrays – indexed 0 = camera 1, 9 = camera 10 ────────────────────
        private static readonly uint[] PresetJoins = {
            JoinMap.CAM_PRESET_1,  JoinMap.CAM_PRESET_2,  JoinMap.CAM_PRESET_3,
            JoinMap.CAM_PRESET_4,  JoinMap.CAM_PRESET_5,  JoinMap.CAM_PRESET_6,
            JoinMap.CAM_PRESET_7,  JoinMap.CAM_PRESET_8,  JoinMap.CAM_PRESET_9,
            JoinMap.CAM_PRESET_10,
        };

        private static readonly uint[] SelectJoins = {
            JoinMap.CAM_SELECT_1,  JoinMap.CAM_SELECT_2,  JoinMap.CAM_SELECT_3,
            JoinMap.CAM_SELECT_4,  JoinMap.CAM_SELECT_5,  JoinMap.CAM_SELECT_6,
            JoinMap.CAM_SELECT_7,  JoinMap.CAM_SELECT_8,  JoinMap.CAM_SELECT_9,
            JoinMap.CAM_SELECT_10,
        };

        private static readonly uint[] ActiveFbJoins = {
            JoinMap.CAM_ACTIVE_FB_1,  JoinMap.CAM_ACTIVE_FB_2,  JoinMap.CAM_ACTIVE_FB_3,
            JoinMap.CAM_ACTIVE_FB_4,  JoinMap.CAM_ACTIVE_FB_5,  JoinMap.CAM_ACTIVE_FB_6,
            JoinMap.CAM_ACTIVE_FB_7,  JoinMap.CAM_ACTIVE_FB_8,  JoinMap.CAM_ACTIVE_FB_9,
            JoinMap.CAM_ACTIVE_FB_10,
        };

        /// <param name="panel">The registered touch-panel device.</param>
        /// <param name="systemInfo">System configuration provider.</param>
        public Camera(BasicTriListWithSmartObject panel, SystemInfo systemInfo)
        {
            _panel      = panel      ?? throw new ArgumentNullException(nameof(panel));
            _systemInfo = systemInfo ?? throw new ArgumentNullException(nameof(systemInfo));
        }

        // ── Pan / Tilt / Zoom (hold) ──────────────────────────────────────────────

        /// <summary>
        /// Called on every press/release event for a PTZ hold join (90–95).
        /// Both edges are forwarded so the camera moves while held and stops on release.
        /// </summary>
        public void HandlePtzPress(uint joinNum, bool isPressed)
        {
            CrestronConsole.PrintLine("[Camera] PTZ {0} {1}", GetPtzLabel(joinNum), isPressed ? "press" : "release");
            // TODO: send serial/IP command to the active camera device
        }

        // ── Power Toggle ──────────────────────────────────────────────────────────

        /// <summary>Toggles camera power and sends feedback to the panel.</summary>
        public void HandlePowerToggle()
        {
            _powerOn = !_powerOn;
            CrestronConsole.PrintLine("[Camera] Power = {0}", _powerOn);
            _panel.BooleanInput[JoinMap.CAM_POWER_FB].BoolValue = _powerOn;
        }

        // ── Tracking Toggle ───────────────────────────────────────────────────────

        /// <summary>Toggles auto-tracking and sends feedback to the panel.</summary>
        public void HandleTrackingToggle()
        {
            _trackingOn = !_trackingOn;
            CrestronConsole.PrintLine("[Camera] Tracking = {0}", _trackingOn);
            _panel.BooleanInput[JoinMap.CAM_TRACKING_FB].BoolValue = _trackingOn;
        }

        // ── Preset Recall ─────────────────────────────────────────────────────────

        /// <summary>Recalls the preset identified by <paramref name="joinNum"/>.</summary>
        public void HandlePresetRecall(uint joinNum)
        {
            int idx = IndexOf(PresetJoins, joinNum);
            if (idx < 0) return;
            CrestronConsole.PrintLine("[Camera] Preset {0} recalled", idx + 1);
            // TODO: send preset recall command to the active camera device
        }

        // ── Camera Select ─────────────────────────────────────────────────────────

        /// <summary>
        /// Selects the camera identified by <paramref name="joinNum"/> and sends
        /// active-feedback and preset names to the panel.
        /// Pressing the same camera again deselects it (reverts to camera 1).
        /// </summary>
        public void HandleCameraSelect(uint joinNum)
        {
            int idx = IndexOf(SelectJoins, joinNum);
            if (idx < 0) return;
            int cameraNumber = idx + 1; // 1-based

            // Clear all active-feedback joins
            for (int i = 0; i < MAX_CAMERAS; i++)
                _panel.BooleanInput[ActiveFbJoins[i]].BoolValue = false;

            // Toggle: deselect (revert to camera 1) if same camera pressed again
            if (cameraNumber == _selectedCamera)
            {
                _selectedCamera = 1;
                _panel.BooleanInput[ActiveFbJoins[_selectedCamera - 1]].BoolValue = true;
                CrestronConsole.PrintLine("[Camera] Camera deselected – reverting to camera 1");
            }
            else
            {
                _selectedCamera = cameraNumber;
                _panel.BooleanInput[ActiveFbJoins[idx]].BoolValue = true;
                CrestronConsole.PrintLine("[Camera] Camera {0} selected", cameraNumber);
                // TODO: switch to the selected camera via the camera switcher
            }

            SendPresetNames(_selectedCamera);
        }

        // ── Zoom Speed ────────────────────────────────────────────────────────────

        /// <summary>Sets the camera zoom speed and echoes feedback to the panel.</summary>
        public void HandleZoomSpeed(ushort value)
        {
            _zoomSpeed = value;
            CrestronConsole.PrintLine("[Camera] Zoom speed = {0}", _zoomSpeed);
            _panel.UShortInput[JoinMap.CAM_ZOOM_SPEED_FB].UShortValue = _zoomSpeed;
        }

        // ── Moment Speed ──────────────────────────────────────────────────────────

        /// <summary>Sets the camera moment (pan/tilt) speed and echoes feedback to the panel.</summary>
        public void HandleMomentSpeed(ushort value)
        {
            _momentSpeed = value;
            CrestronConsole.PrintLine("[Camera] Moment speed = {0}", _momentSpeed);
            _panel.UShortInput[JoinMap.CAM_MOMENT_SPEED_FB].UShortValue = _momentSpeed;
        }

        // ── Camera Names ──────────────────────────────────────────────────────────

        /// <summary>
        /// Sends all camera names from the loaded configuration to the panel
        /// (serial joins 34–43). Empty string is sent for unused slots.
        /// </summary>
        public void SendCameraNames()
        {
            List<SystemInfo.Camera> cameras = _systemInfo.Config?.Cameras;
            for (int i = 0; i < MAX_CAMERAS; i++)
            {
                string name = (cameras != null && i < cameras.Count) ? cameras[i].Name ?? string.Empty : string.Empty;
                _panel.StringInput[JoinMap.CAM_NAME_SERIALS[i]].StringValue = name;
            }
            CrestronConsole.PrintLine("[Camera] Camera names sent to panel");
        }

        // ── Preset Names ──────────────────────────────────────────────────────────

        /// <summary>
        /// Sends the preset names for the specified camera (1-based) to the panel
        /// (serial joins 44–53). Empty string is sent for unused preset slots.
        /// </summary>
        private void SendPresetNames(int cameraNumber)
        {
            List<SystemInfo.Camera> cameras = _systemInfo.Config?.Cameras;
            List<SystemInfo.CameraPreset> presets = null;

            if (cameras != null && cameraNumber >= 1 && cameraNumber <= cameras.Count)
                presets = cameras[cameraNumber - 1].Presets;

            for (int i = 0; i < MAX_PRESETS; i++)
            {
                string name = (presets != null && i < presets.Count) ? presets[i].Name ?? string.Empty : string.Empty;
                _panel.StringInput[JoinMap.CAM_PRESET_NAME_SERIALS[i]].StringValue = name;
            }
            CrestronConsole.PrintLine("[Camera] Preset names for camera {0} sent to panel", cameraNumber);
        }

        // ── Refresh / Initial Push ────────────────────────────────────────────────

        /// <summary>
        /// Pushes the current camera state to the panel, sends all camera names,
        /// and sends preset names for the selected camera (defaults to camera 1).
        /// Call after the panel comes online or when config is (re)loaded.
        /// </summary>
        public void RefreshAll()
        {
            _panel.BooleanInput[JoinMap.CAM_POWER_FB].BoolValue    = _powerOn;
            _panel.BooleanInput[JoinMap.CAM_TRACKING_FB].BoolValue = _trackingOn;

            for (int i = 0; i < MAX_CAMERAS; i++)
                _panel.BooleanInput[ActiveFbJoins[i]].BoolValue = (i + 1 == _selectedCamera);

            _panel.UShortInput[JoinMap.CAM_ZOOM_SPEED_FB].UShortValue   = _zoomSpeed;
            _panel.UShortInput[JoinMap.CAM_MOMENT_SPEED_FB].UShortValue = _momentSpeed;

            SendCameraNames();
            SendPresetNames(_selectedCamera);
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private static int IndexOf(uint[] joins, uint joinNum)
        {
            for (int i = 0; i < joins.Length; i++)
                if (joins[i] == joinNum) return i;
            return -1;
        }

        private static string GetPtzLabel(uint joinNum)
        {
            if (joinNum == JoinMap.CAM_PAN_LEFT_BTN)  return "Pan Left";
            if (joinNum == JoinMap.CAM_PAN_RIGHT_BTN) return "Pan Right";
            if (joinNum == JoinMap.CAM_TILT_UP_BTN)   return "Tilt Up";
            if (joinNum == JoinMap.CAM_TILT_DOWN_BTN) return "Tilt Down";
            if (joinNum == JoinMap.CAM_ZOOM_IN_BTN)   return "Zoom In";
            if (joinNum == JoinMap.CAM_ZOOM_OUT_BTN)  return "Zoom Out";
            return "Unknown";
        }
    }
}
