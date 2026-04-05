using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.UI;
using System;
using static ExecutiveControlSystem.SystemInfo;

namespace ExecutiveControlSystem
{
    /// <summary>
    /// Encapsulates all room-control logic for the executive panel.
    /// Handles lighting, volume, video routing, Teams/BYOD modes, and the
    /// one-touch executive scenario macros (System Startup, System Off,
    /// Present to Room).
    /// </summary>
    public class RoomController
    {
        private readonly BasicTriListWithSmartObject _panel;
        private readonly Video       _video;
        private readonly Audio       _audio;
        private readonly Camera      _camera;
        private readonly SystemInfo  _systemInfo;
        private readonly UserConfig  _userConfig;

        private const ushort MASTER_DEFAULT_VOLUME = 32767; // 50 % of 65 535

        private bool   _lightsOn;
        private ushort _volume;
        private bool   _masterMuted;
        private ushort _brightness;
        private bool   _roomIsOn;
        private bool   _teamsModeOn;
        private bool   _byodModeOn;

        // Destination power / video state (4 destinations)
        private readonly bool[] _destPowerOn = new bool[4];
        private readonly bool[] _destVideoOn  = new bool[4];

        // Join arrays for destination power/video feedback
        private static readonly uint[] DestPowerFbJoins = {
            JoinMap.VIDEO_DEST_POWER_FB_1, JoinMap.VIDEO_DEST_POWER_FB_2,
            JoinMap.VIDEO_DEST_POWER_FB_3, JoinMap.VIDEO_DEST_POWER_FB_4,
        };

        private static readonly uint[] DestVideoFbJoins = {
            JoinMap.VIDEO_DEST_VIDEO_FB_1, JoinMap.VIDEO_DEST_VIDEO_FB_2,
            JoinMap.VIDEO_DEST_VIDEO_FB_3, JoinMap.VIDEO_DEST_VIDEO_FB_4,
        };

        /// <summary>Gets or sets the last selected video source ID (1-based).</summary>
        public ushort LastSelectedSourceId { get; set; }

        /// <summary>Gets or sets the last selected video destination ID (1-based).</summary>
        public ushort LastSelectedDestinationId { get; set; }

        /// <param name="panel">The registered touch-panel device.</param>
        public RoomController(BasicTriListWithSmartObject panel)
        {
            _panel      = panel ?? throw new ArgumentNullException(nameof(panel));
            _systemInfo = new SystemInfo(panel);
            _userConfig = new UserConfig(panel);
            _video      = new Video(panel, this, _systemInfo);
            _audio      = new Audio(panel, _systemInfo);
            _camera     = new Camera(panel, _systemInfo);
        }

        // ── Executive scenario macros ─────────────────────────────────────────────

        /// <summary>
        /// Powers on the room: turns on all displays, enables audio, and sends
        /// <c>SYSTEM_OFF_FB</c> high so the panel shows the room as active.
        /// </summary>
        public void SystemStartup()
        {
            if (_roomIsOn)
            {
                CrestronConsole.PrintLine("[RoomController] SystemStartup – room already on");
                return;
            }

            _roomIsOn = true;
            CrestronConsole.PrintLine("[RoomController] System startup initiated");

            // Power on all destinations
            for (int i = 0; i < 4; i++)
            {
                _destPowerOn[i] = true;
                _destVideoOn[i] = true;
                _panel.BooleanInput[DestPowerFbJoins[i]].BoolValue = true;
                _panel.BooleanInput[DestVideoFbJoins[i]].BoolValue = true;
            }

            SendSystemOnFeedback();
        }

        /// <summary>
        /// Powers down the room: turns off all displays, clears source/destination
        /// selections, and sends <c>SYSTEM_OFF_FB</c> low.
        /// </summary>
        public void ShutdownSystem()
        {
            if (!_roomIsOn)
            {
                CrestronConsole.PrintLine("[RoomController] ShutdownSystem – room already off");
                return;
            }

            _roomIsOn = false;
            CrestronConsole.PrintLine("[RoomController] System shutdown initiated");

            // Power off all destinations
            for (int i = 0; i < 4; i++)
            {
                _destPowerOn[i] = false;
                _destVideoOn[i] = false;
                _panel.BooleanInput[DestPowerFbJoins[i]].BoolValue = false;
                _panel.BooleanInput[DestVideoFbJoins[i]].BoolValue = false;
            }

            // Clear source / destination selections
            LastSelectedSourceId      = 0;
            LastSelectedDestinationId = 0;
            for (uint i = 0; i < 5; i++)
                _panel.BooleanInput[JoinMap.VIDEO_SRC_ACTIVE_1 + i].BoolValue = false;
            for (uint i = 0; i < 4; i++)
                _panel.BooleanInput[JoinMap.VIDEO_DEST_ACTIVE_1 + i].BoolValue = false;

            // Clear Teams / BYOD modes
            _teamsModeOn = false;
            _byodModeOn  = false;
            _panel.BooleanInput[JoinMap.TEAMS_MODE_FB].BoolValue = false;
            _panel.BooleanInput[JoinMap.BYOD_MODE_FB].BoolValue  = false;

            SendSystemOnFeedback();
        }

        /// <summary>
        /// Routes the first available video source (typically the local PC) to
        /// all displays and powers them on – the executive "one-touch present" macro.
        /// </summary>
        public void PresentToRoom()
        {
            CrestronConsole.PrintLine("[RoomController] Present to Room");

            if (!_roomIsOn)
                SystemStartup();

            // Select source 1 (local PC) for every destination
            LastSelectedSourceId = 1;

            // Clear all source active feedback, then set source 1
            for (uint i = 0; i < 5; i++)
                _panel.BooleanInput[JoinMap.VIDEO_SRC_ACTIVE_1 + i].BoolValue = (i == 0);

            // Route source 1 to all destinations and power them on
            for (ushort dest = 1; dest <= 4; dest++)
            {
                LastSelectedDestinationId = dest;
                _video.VideoRouting();

                int idx = dest - 1;
                if (!_destPowerOn[idx])
                {
                    _destPowerOn[idx] = true;
                    _panel.BooleanInput[DestPowerFbJoins[idx]].BoolValue = true;
                }
                if (!_destVideoOn[idx])
                {
                    _destVideoOn[idx] = true;
                    _panel.BooleanInput[DestVideoFbJoins[idx]].BoolValue = true;
                }
            }

            LastSelectedDestinationId = 0;
            for (uint i = 0; i < 4; i++)
                _panel.BooleanInput[JoinMap.VIDEO_DEST_ACTIVE_1 + i].BoolValue = false;
        }

        private void SendSystemOnFeedback()
        {
            _panel.BooleanInput[JoinMap.SYSTEM_OFF_FB].BoolValue = _roomIsOn;
        }

        // ── Teams / BYOD mode toggles ────────────────────────────────────────────

        /// <summary>Toggles Teams meeting mode and sends feedback to the panel.</summary>
        public void TeamsModeToggle()
        {
            _teamsModeOn = !_teamsModeOn;
            if (_teamsModeOn)
                _byodModeOn = false; // mutually exclusive with BYOD

            CrestronConsole.PrintLine("[RoomController] Teams mode = {0}", _teamsModeOn);
            _panel.BooleanInput[JoinMap.TEAMS_MODE_FB].BoolValue = _teamsModeOn;
            _panel.BooleanInput[JoinMap.BYOD_MODE_FB].BoolValue  = _byodModeOn;
        }

        /// <summary>Toggles BYOD mode and sends feedback to the panel.</summary>
        public void ByodModeToggle()
        {
            _byodModeOn = !_byodModeOn;
            if (_byodModeOn)
                _teamsModeOn = false; // mutually exclusive with Teams

            CrestronConsole.PrintLine("[RoomController] BYOD mode = {0}", _byodModeOn);
            _panel.BooleanInput[JoinMap.TEAMS_MODE_FB].BoolValue = _teamsModeOn;
            _panel.BooleanInput[JoinMap.BYOD_MODE_FB].BoolValue  = _byodModeOn;
        }

        // ── Lighting ─────────────────────────────────────────────────────────────

        /// <summary>Turn the room lights on.</summary>
        public void LightsOn()
        {
            _lightsOn = true;
            CrestronConsole.PrintLine("[RoomController] Lights ON");
            SendLightFeedback();
        }

        /// <summary>Turn the room lights off.</summary>
        public void LightsOff()
        {
            _lightsOn = false;
            CrestronConsole.PrintLine("[RoomController] Lights OFF");
            SendLightFeedback();
        }

        /// <summary>Toggle the room lights.</summary>
        public void LightsToggle()
        {
            _lightsOn = !_lightsOn;
            CrestronConsole.PrintLine("[RoomController] Lights {0}", _lightsOn ? "ON" : "OFF");
            SendLightFeedback();
        }

        private void SendLightFeedback()
        {
            _panel.BooleanInput[JoinMap.LIGHT_IS_ON].BoolValue = _lightsOn;
        }

        /// <summary>Sets the display brightness and echoes feedback to the panel.</summary>
        public void SetBrightness(ushort brightness)
        {
            _brightness = brightness;
            CrestronConsole.PrintLine("[RoomController] Brightness set to {0}", _brightness);
            _panel.UShortInput[JoinMap.BRIGHTNESS_FB].UShortValue = _brightness;
        }

        // ── Video source selection ────────────────────────────────────────────────

        /// <summary>Selects or deselects a video source and triggers routing if a destination is also selected.</summary>
        public void SendSourceSelectFeedback(ushort sourceId)
        {
            for (ushort i = 1; i <= 5; i++)
                _panel.BooleanInput[JoinMap.VIDEO_SRC_ACTIVE_1 + i - 1].BoolValue = false;

            if (sourceId != LastSelectedSourceId)
            {
                _panel.BooleanInput[JoinMap.VIDEO_SRC_ACTIVE_1 + sourceId - 1].BoolValue = true;
                LastSelectedSourceId = sourceId;
                CrestronConsole.PrintLine("[RoomController] Source {0} selected", sourceId);
                _video.VideoRouting();
            }
            else
            {
                LastSelectedSourceId = 0;
                CrestronConsole.PrintLine("[RoomController] Source deselected");
            }
        }

        // ── Video destination selection ───────────────────────────────────────────

        /// <summary>Selects or deselects a video destination and triggers routing if a source is also selected.</summary>
        public void SendDestinationSelectFeedback(ushort destinationId)
        {
            for (ushort i = 1; i <= 4; i++)
                _panel.BooleanInput[JoinMap.VIDEO_DEST_ACTIVE_1 + i - 1].BoolValue = false;

            if (destinationId != LastSelectedDestinationId)
            {
                _panel.BooleanInput[JoinMap.VIDEO_DEST_ACTIVE_1 + destinationId - 1].BoolValue = true;
                LastSelectedDestinationId = destinationId;
                CrestronConsole.PrintLine("[RoomController] Destination {0} selected", destinationId);
                _video.VideoRouting();
            }
            else
            {
                LastSelectedDestinationId = 0;
                CrestronConsole.PrintLine("[RoomController] Destination deselected");
            }
        }

        // ── Destination power / video toggles ────────────────────────────────────

        /// <summary>Toggles the power state of a display destination (1-based).</summary>
        public void DestinationPowerToggle(ushort destId)
        {
            if (destId < 1 || destId > 4) return;
            int idx = destId - 1;
            _destPowerOn[idx] = !_destPowerOn[idx];
            CrestronConsole.PrintLine("[RoomController] Destination {0} power = {1}", destId, _destPowerOn[idx]);
            _panel.BooleanInput[DestPowerFbJoins[idx]].BoolValue = _destPowerOn[idx];
        }

        /// <summary>Toggles the video output of a display destination (1-based).</summary>
        public void DestinationVideoToggle(ushort destId)
        {
            if (destId < 1 || destId > 4) return;
            int idx = destId - 1;
            _destVideoOn[idx] = !_destVideoOn[idx];
            CrestronConsole.PrintLine("[RoomController] Destination {0} video = {1}", destId, _destVideoOn[idx]);
            _panel.BooleanInput[DestVideoFbJoins[idx]].BoolValue = _destVideoOn[idx];
        }

        // ── Master volume ─────────────────────────────────────────────────────────

        /// <summary>Sets the master volume level and echoes feedback to the panel.</summary>
        public void SetVolume(ushort level)
        {
            _volume = level;
            CrestronConsole.PrintLine("[RoomController] Master volume set to {0}", _volume);
            _panel.UShortInput[JoinMap.VOLUME_FB].UShortValue = _volume;
        }

        /// <summary>Restores the master volume to the default level (50 %).</summary>
        public void MasterVolDefault()
        {
            CrestronConsole.PrintLine("[RoomController] Master volume reset to default");
            SetVolume(MASTER_DEFAULT_VOLUME);
        }

        /// <summary>Toggles master volume mute and sends feedback to the panel.</summary>
        public void MasterVolMuteToggle()
        {
            _masterMuted = !_masterMuted;
            CrestronConsole.PrintLine("[RoomController] Master volume mute = {0}", _masterMuted);
            _panel.BooleanInput[JoinMap.MASTER_VOL_MUTE_FB].BoolValue = _masterMuted;
        }

        // ── Source name feedback ──────────────────────────────────────────────────

        /// <summary>Sends video source names from the loaded JSON configuration to the panel.</summary>
        private void SendSourceNameFeedback()
        {
            if (_systemInfo.Config?.VideoSources == null)
            {
                CrestronConsole.PrintLine("[RoomController] Cannot send source names – config not loaded");
                return;
            }

            for (int i = 0; i < _systemInfo.Config.VideoSources.Count && i < 5; i++)
                _panel.StringInput[JoinMap.VIDEO_SRC_NAME_1 + (uint)i].StringValue = _systemInfo.Config.VideoSources[i].Name ?? string.Empty;
        }

        /// <summary>Sends video destination names from the loaded JSON configuration to the panel.</summary>
        private void SendDestinationNameFeedback()
        {
            if (_systemInfo.Config?.VideoDestinations == null)
            {
                CrestronConsole.PrintLine("[RoomController] Cannot send destination names – config not loaded");
                return;
            }

            for (int i = 0; i < _systemInfo.Config.VideoDestinations.Count && i < 4; i++)
                _panel.StringInput[JoinMap.VIDEO_DEST_NAME_1 + (uint)i].StringValue = _systemInfo.Config.VideoDestinations[i].Name ?? string.Empty;
        }

        // ── Audio delegation ──────────────────────────────────────────────────────

        /// <summary>Delegates a volume-up press/release event to the Audio handler.</summary>
        public void AudioVolUpPress(uint joinNum, bool isPressed) => _audio.HandleVolUpPress(joinNum, isPressed);

        /// <summary>Delegates a volume-down press/release event to the Audio handler.</summary>
        public void AudioVolDownPress(uint joinNum, bool isPressed) => _audio.HandleVolDownPress(joinNum, isPressed);

        /// <summary>Delegates a default-volume pulse to the Audio handler.</summary>
        public void AudioDefault(uint joinNum) => _audio.HandleDefault(joinNum);

        /// <summary>Delegates a mute-toggle pulse to the Audio handler.</summary>
        public void AudioMuteToggle(uint joinNum) => _audio.HandleMuteToggle(joinNum);

        /// <summary>Delegates an absolute volume-set analog value to the Audio handler.</summary>
        public void AudioVolSet(uint joinNum, ushort value) => _audio.HandleVolSet(joinNum, value);

        /// <summary>Delegates a privacy-mute toggle to the Audio handler.</summary>
        public void AudioPrivacyMuteToggle() => _audio.HandlePrivacyMuteToggle();

        /// <summary>Delegates a wireless-mic mute toggle to the Audio handler.</summary>
        public void AudioWirelessMicMuteToggle() => _audio.HandleWirelessMicMuteToggle();

        /// <summary>Delegates a ceiling-mic mute toggle to the Audio handler.</summary>
        public void AudioCeilingMicMuteToggle() => _audio.HandleCeilingMicMuteToggle();

        // ── Camera delegation ─────────────────────────────────────────────────────

        /// <summary>Delegates a PTZ hold press/release event to the Camera handler.</summary>
        public void CameraPtzPress(uint joinNum, bool isPressed) => _camera.HandlePtzPress(joinNum, isPressed);

        /// <summary>Delegates a power-toggle pulse to the Camera handler.</summary>
        public void CameraPowerToggle() => _camera.HandlePowerToggle();

        /// <summary>Delegates a tracking-toggle pulse to the Camera handler.</summary>
        public void CameraTrackingToggle() => _camera.HandleTrackingToggle();

        /// <summary>Delegates a preset-recall pulse to the Camera handler.</summary>
        public void CameraPresetRecall(uint joinNum) => _camera.HandlePresetRecall(joinNum);

        /// <summary>Delegates a camera-select pulse to the Camera handler.</summary>
        public void CameraSelect(uint joinNum) => _camera.HandleCameraSelect(joinNum);

        /// <summary>Delegates a zoom-speed analog value to the Camera handler.</summary>
        public void CameraZoomSpeed(ushort value) => _camera.HandleZoomSpeed(value);

        /// <summary>Delegates a moment-speed analog value to the Camera handler.</summary>
        public void CameraMomentSpeed(ushort value) => _camera.HandleMomentSpeed(value);

        // ── Initial state push ───────────────────────────────────────────────────

        /// <summary>
        /// Pushes the current state to the panel (called when the panel comes online).
        /// </summary>
        public void RefreshAll()
        {
            SendLightFeedback();
            _panel.UShortInput[JoinMap.VOLUME_FB].UShortValue    = _volume;
            _panel.BooleanInput[JoinMap.MASTER_VOL_MUTE_FB].BoolValue = _masterMuted;
            _panel.UShortInput[JoinMap.BRIGHTNESS_FB].UShortValue = _brightness;

            SendSystemOnFeedback();
            _panel.BooleanInput[JoinMap.TEAMS_MODE_FB].BoolValue = _teamsModeOn;
            _panel.BooleanInput[JoinMap.BYOD_MODE_FB].BoolValue  = _byodModeOn;

            for (int i = 0; i < 4; i++)
            {
                _panel.BooleanInput[DestPowerFbJoins[i]].BoolValue = _destPowerOn[i];
                _panel.BooleanInput[DestVideoFbJoins[i]].BoolValue = _destVideoOn[i];
            }

            _systemInfo.LoadSystemConfig();
            _systemInfo.PushNetworkInfo();
            _userConfig.LoadAndPush();
            _systemInfo.ClockFormat = _userConfig.Data.ClockFormat ?? "12h";
            _systemInfo.StartDateTimeUpdates();

            SendSourceNameFeedback();
            SendDestinationNameFeedback();

            _audio.RefreshAll();
            _camera.RefreshAll();
        }

        /// <summary>Stops date/time updates (called when the panel goes offline).</summary>
        public void StopSystemInfo()
        {
            _systemInfo.StopDateTimeUpdates();
        }

        // ── User settings delegation ─────────────────────────────────────────────

        /// <summary>Resets all user settings to defaults and pushes them to the panel.</summary>
        public void SettingsReset() => _userConfig.ResetToDefaults();

        /// <summary>Saves an updated theme-mode string from the panel.</summary>
        public void SettingsThemeMode(string value) => _userConfig.HandleThemeMode(value);

        /// <summary>Saves an updated brand-colour string from the panel.</summary>
        public void SettingsBrandColor(string value) => _userConfig.HandleBrandColor(value);

        /// <summary>Saves an updated clock-format string from the panel and immediately refreshes the time display.</summary>
        public void SettingsClockFormat(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            _userConfig.HandleClockFormat(value);
            _systemInfo.ClockFormat = value;
            _systemInfo.UpdateDateTime();
        }

        /// <summary>Saves an updated temperature-unit string from the panel.</summary>
        public void SettingsTempUnit(string value) => _userConfig.HandleTempUnit(value);

        /// <summary>Saves an updated startup volume analog value from the panel.</summary>
        public void SettingsStartupVolume(ushort rawValue) => _userConfig.HandleStartupVolume(rawValue);

        /// <summary>Toggles the BYOD auto-switch setting.</summary>
        public void SettingsByodAutoSwitchToggle() => _userConfig.ToggleByodAutoSwitch();

        /// <summary>Toggles the BYOD auto-power-on setting.</summary>
        public void SettingsByodAutoPowerToggle() => _userConfig.ToggleByodAutoPowerOn();
    }
}
