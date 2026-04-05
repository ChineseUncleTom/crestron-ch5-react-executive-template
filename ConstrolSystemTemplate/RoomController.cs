using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.UI;
using System;
using static ConstrolSystemTemplate.SystemInfo;

namespace ConstrolSystemTemplate
{
    /// <summary>
    /// Encapsulates all room-control logic: lighting, volume, and source selection.
    /// RoomController receives commands from the panel through the join callbacks
    /// registered in ControlSystem and sends feedback back to the panel.
    /// </summary>
    public class RoomController
    {
        private readonly BasicTriListWithSmartObject _panel;
        private readonly Video _video;
        private readonly Audio _audio;
        private readonly Camera _camera;
        private readonly SystemInfo _systemInfo;
        private bool   _lightsOn;
        private ushort _volume;
        private ushort _brightness;
        private string _sourceName;
        private RoomConfig config;

        /// <summary>Gets or sets the last selected video source ID.</summary>
        public ushort LastSelectedSourceId { get; set; }

        /// <summary>Gets or sets the last selected video destination ID.</summary>
        public ushort LastSelectedDestinationId { get; set; }

        /// <param name="panel">The registered touch-panel device.</param>
        public RoomController(BasicTriListWithSmartObject panel)
        {
            _panel      = panel ?? throw new ArgumentNullException(nameof(panel));
            _systemInfo = new SystemInfo(panel);
            _video      = new Video(panel, this, _systemInfo);
            _audio      = new Audio(panel, _systemInfo);
            _camera     = new Camera(panel, _systemInfo);
            _lightsOn   = false;
            _volume     = 0;
            _brightness = 0;
            _sourceName = "No Source";
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

        public void LightsToggle()
        {
            _lightsOn = !_lightsOn;
            if (_lightsOn)
            {
                CrestronConsole.PrintLine("[RoomController] Lights ON");
            }
            else
            {
                CrestronConsole.PrintLine("[RoomController] Lights OFF");
            }
            SendLightFeedback();
        }

        private void SendLightFeedback()
        {
            _panel.BooleanInput[JoinMap.LIGHT_IS_ON].BoolValue = _lightsOn;
        }

        public void SetBrightness(ushort brightness)
        {
            _brightness = brightness;
            CrestronConsole.PrintLine("[RoomController] Brightness set to {0}", _brightness);
            SendBrightnessFeedback();
        }

        public void SendBrightnessFeedback()
        {
            _panel.UShortInput[JoinMap.BRIGHTNESS_FB].UShortValue = _brightness;
        }

        // ── Source selection ─────────────────────────────────────────────────────
        public void SendSourceSelectFeedback(ushort sourceId)
        {
            // Clear all source feedback joins
            for (ushort i = 1; i <= 5; i++)
            {
                _panel.BooleanInput[JoinMap.VIDEO_SRC_ACTIVE_1 + i - 1].BoolValue = false;
            }

            // Toggle source selection
            if (sourceId != LastSelectedSourceId)
            {
                _panel.BooleanInput[JoinMap.VIDEO_SRC_ACTIVE_1 + sourceId - 1].BoolValue = true;
                LastSelectedSourceId = sourceId;
                CrestronConsole.PrintLine("[RoomController] Source {0} selected", sourceId);

                // Perform routing if destination is also selected
                _video.VideoRouting();
            }
            else
            {
                // Deselect if same source pressed again
                LastSelectedSourceId = 0;
                CrestronConsole.PrintLine("[RoomController] Source deselected");
            }
        }

        // ── Destination selection ─────────────────────────────────────────────────────
        public void SendDestinationSelectFeedback(ushort destinationId)
        {
            // Clear all destination feedback joins
            for (ushort i = 1; i <= 4; i++)
            {
                _panel.BooleanInput[JoinMap.VIDEO_DEST_ACTIVE_1 + i - 1].BoolValue = false;
            }

            // Toggle destination selection
            if (destinationId != LastSelectedDestinationId)
            {
                _panel.BooleanInput[JoinMap.VIDEO_DEST_ACTIVE_1 + destinationId - 1].BoolValue = true;
                LastSelectedDestinationId = destinationId;
                CrestronConsole.PrintLine("[RoomController] Destination {0} selected", destinationId);

                // Perform routing if source is also selected
                _video.VideoRouting();
            }
            else
            {
                // Deselect if same destination pressed again
                LastSelectedDestinationId = 0;
                CrestronConsole.PrintLine("[RoomController] Destination deselected");
            }
        }

        // ── Volume ───────────────────────────────────────────────────────────────

        /// <summary>Set the volume level and echo feedback back to the panel.</summary>
        /// <param name="level">Raw analog value 0–65 535.</param>
        public void SetVolume(ushort level)
        {
            _volume = level;
            CrestronConsole.PrintLine("[RoomController] Volume set to {0}", _volume);
            SendVolumeFeedback();
        }

        private void SendVolumeFeedback()
        {
            _panel.UShortInput[JoinMap.VOLUME_FB].UShortValue = _volume;
        }

        // ── Source selection ─────────────────────────────────────────────────────

        /// <summary>Update the displayed source name on the panel.</summary>
        /// <param name="name">Human-readable source name, e.g. "Apple TV".</param>
        public void SetSourceName(string name)
        {
            _sourceName = name ?? string.Empty;
            CrestronConsole.PrintLine("[RoomController] Source = {0}", _sourceName);
            SendSourceFeedback();
        }

        private void SendSourceFeedback()
        {
            // Check if config is loaded
            if (_systemInfo.Config == null || _systemInfo.Config.VideoSources == null)
            {
                CrestronConsole.PrintLine("[RoomController] Cannot send source feedback - Config not loaded");
                return;
            }

            // Send each video source name to the panel
            for (int i = 0; i < _systemInfo.Config.VideoSources.Count && i < 5; i++)
            {
                _panel.StringInput[JoinMap.VIDEO_SRC_NAME_1 + (uint)i].StringValue = _systemInfo.Config.VideoSources[i].Name;
            }
        }

        // ── Audio ────────────────────────────────────────────────────────────────

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

        /// <summary>Delegates a master-volume mute toggle to the Audio handler.</summary>
        public void AudioMasterVolMuteToggle() => _audio.HandleMasterVolMuteToggle();

        /// <summary>Resets the master volume to the default level (50%).</summary>
        public void AudioMasterVolDefault() => SetVolume(32767); // 32767 ≈ 50% of 65535

        // ── Camera ───────────────────────────────────────────────────────────────

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
        /// Push the current state to the panel (call after the panel comes online).
        /// </summary>
        public void RefreshAll()
        {
            SendLightFeedback();
            SendVolumeFeedback();
            SendSourceFeedback();
            _systemInfo.LoadSystemConfig();
            _systemInfo.StartDateTimeUpdates();
            _audio.RefreshAll();
            _camera.RefreshAll();
        }

        /// <summary>
        /// Stops system information updates (call when panel goes offline).
        /// </summary>
        public void StopSystemInfo()
        {
            _systemInfo.StopDateTimeUpdates();
        }
    }
}
