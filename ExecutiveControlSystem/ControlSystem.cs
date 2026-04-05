using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.UI;

namespace ExecutiveControlSystem
{
    /// <summary>
    /// SIMPL# Pro entry point for the Crestron CH5 Executive starter project.
    /// Registers the touch panel, wires join callbacks, and delegates
    /// all room logic to <see cref="RoomController"/>.
    /// </summary>
    public class ControlSystem : CrestronControlSystem
    {
        // Change this to match the IP-ID configured on your touch panel.
        private const uint PANEL_IPID = 0x03;

        private XpanelForHtml5 _panel;
        private RoomController _room;

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
            }
            catch (Exception e)
            {
                ErrorLog.Error("[ControlSystem] Constructor exception: {0}", e.Message);
            }
        }

        /// <summary>Called by the framework when the program starts.</summary>
        public override void InitializeSystem()
        {
            try
            {
                // ── Register the touch panel ─────────────────────────────────────
                _panel = new XpanelForHtml5(PANEL_IPID, this);
                _panel.Description = "Executive Room Panel";

                // ── Wire join events ─────────────────────────────────────────────
                _panel.SigChange += OnPanelSigChange;
                _panel.OnlineStatusChange += OnPanelOnlineStatusChange;

                // ── Register with the control system ─────────────────────────────
                if (_panel.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("[ControlSystem] Failed to register panel on IP-ID 0x{0:X2}", PANEL_IPID);
                    return;
                }

                // ── Create room controller ────────────────────────────────────────
                _room = new RoomController(_panel);

                CrestronConsole.PrintLine("[ControlSystem] Initialized – waiting for panel on IP-ID 0x{0:X2}", PANEL_IPID);
            }
            catch (Exception e)
            {
                ErrorLog.Error("[ControlSystem] InitializeSystem exception: {0}", e.Message);
            }
        }

        // ── Panel online / offline ────────────────────────────────────────────────

        private void OnPanelOnlineStatusChange(GenericBase device, OnlineOfflineEventArgs args)
        {
            if (args.DeviceOnLine)
            {
                CrestronConsole.PrintLine("[ControlSystem] Panel online – pushing initial state");
                _room.RefreshAll();
            }
            else
            {
                CrestronConsole.PrintLine("[ControlSystem] Panel offline");
                _room.StopSystemInfo();
            }
        }

        // ── Join change handler ───────────────────────────────────────────────────

        private void OnPanelSigChange(BasicTriList device, SigEventArgs args)
        {
            var sig = args.Sig;

            switch (sig.Type)
            {
                case eSigType.Bool:
                    HandleDigitalJoin(sig);
                    break;

                case eSigType.UShort:
                    HandleAnalogJoin(sig);
                    break;

                case eSigType.String:
                    HandleSerialJoin(sig);
                    break;

                default:
                    break;
            }
        }

        private void HandleDigitalJoin(Sig sig)
        {
            uint joinNum  = sig.Number;
            bool isPressed = sig.BoolValue;

            // ── Audio source volume-up buttons (joins 59–63, hold – handle both edges) ──
            if (joinNum >= JoinMap.AUDIO_SRC_VOL_UP_1 && joinNum <= JoinMap.AUDIO_SRC_VOL_UP_5)
            {
                _room.AudioVolUpPress(joinNum, isPressed);
                return;
            }

            // ── Audio source volume-down buttons (joins 64–68, hold – handle both edges) ─
            if (joinNum >= JoinMap.AUDIO_SRC_VOL_DOWN_1 && joinNum <= JoinMap.AUDIO_SRC_VOL_DOWN_5)
            {
                _room.AudioVolDownPress(joinNum, isPressed);
                return;
            }

            // ── Camera PTZ buttons (joins 90–95, hold – handle both edges) ────────
            if (joinNum >= JoinMap.CAM_PAN_LEFT_BTN && joinNum <= JoinMap.CAM_ZOOM_OUT_BTN)
            {
                _room.CameraPtzPress(joinNum, isPressed);
                return;
            }

            // Only act on rising edge (button press) for all remaining joins.
            if (!isPressed)
                return;

            // ── Executive scenario macros ────────────────────────────────────────

            if (joinNum == JoinMap.SYSTEM_STARTUP_BTN)
            {
                _room.SystemStartup();
                return;
            }

            if (joinNum == JoinMap.SYSTEM_OFF_BTN)
            {
                _room.ShutdownSystem();
                return;
            }

            if (joinNum == JoinMap.PRESENT_TO_ROOM_BTN)
            {
                _room.PresentToRoom();
                return;
            }

            // ── Lighting ─────────────────────────────────────────────────────────

            if (joinNum == JoinMap.LIGHT_TOGGLE)
            {
                _room.LightsToggle();
                return;
            }

            // ── Teams / BYOD mode toggles ────────────────────────────────────────

            if (joinNum == JoinMap.TEAMS_MODE_BTN)
            {
                _room.TeamsModeToggle();
                return;
            }

            if (joinNum == JoinMap.BYOD_MODE_BTN)
            {
                _room.ByodModeToggle();
                return;
            }

            if (joinNum == JoinMap.BYOD_SELECT_BTN)
            {
                CrestronConsole.PrintLine("[ControlSystem] BYOD select");
                return;
            }

            // ── Video source selection (joins 11–15) ─────────────────────────────
            if (joinNum >= JoinMap.VIDEO_SRC_SELECT_1 && joinNum <= JoinMap.VIDEO_SRC_SELECT_5)
            {
                ushort sourceId = (ushort)(joinNum - JoinMap.VIDEO_SRC_SELECT_1 + 1);
                _room.SendSourceSelectFeedback(sourceId);
                return;
            }

            // ── Video destination selection (joins 46–49) ────────────────────────
            if (joinNum >= JoinMap.VIDEO_DEST_SELECT_1 && joinNum <= JoinMap.VIDEO_DEST_SELECT_4)
            {
                ushort destId = (ushort)(joinNum - JoinMap.VIDEO_DEST_SELECT_1 + 1);
                _room.SendDestinationSelectFeedback(destId);
                return;
            }

            // ── Destination power buttons (joins 21–24) ──────────────────────────
            if (joinNum >= JoinMap.VIDEO_DEST_POWER_BTN_1 && joinNum <= JoinMap.VIDEO_DEST_POWER_BTN_4)
            {
                ushort destId = (ushort)(joinNum - JoinMap.VIDEO_DEST_POWER_BTN_1 + 1);
                _room.DestinationPowerToggle(destId);
                return;
            }

            // ── Destination video buttons (joins 31–34) ──────────────────────────
            if (joinNum >= JoinMap.VIDEO_DEST_VIDEO_BTN_1 && joinNum <= JoinMap.VIDEO_DEST_VIDEO_BTN_4)
            {
                ushort destId = (ushort)(joinNum - JoinMap.VIDEO_DEST_VIDEO_BTN_1 + 1);
                _room.DestinationVideoToggle(destId);
                return;
            }

            // ── Audio source default-volume buttons (joins 69–73) ────────────────
            if (joinNum >= JoinMap.AUDIO_SRC_DEFAULT_1 && joinNum <= JoinMap.AUDIO_SRC_DEFAULT_5)
            {
                _room.AudioDefault(joinNum);
                return;
            }

            // ── Audio source mute-toggle buttons (joins 74–78) ───────────────────
            if (joinNum >= JoinMap.AUDIO_SRC_MUTE_BTN_1 && joinNum <= JoinMap.AUDIO_SRC_MUTE_BTN_5)
            {
                _room.AudioMuteToggle(joinNum);
                return;
            }

            // ── Far-end audio mute buttons (joins 84–86) ─────────────────────────
            if (joinNum == JoinMap.AUDIO_PRIVACY_MUTE_BTN)
            {
                _room.AudioPrivacyMuteToggle();
                return;
            }

            if (joinNum == JoinMap.AUDIO_WIRELESS_MIC_MUTE_BTN)
            {
                _room.AudioWirelessMicMuteToggle();
                return;
            }

            if (joinNum == JoinMap.AUDIO_CEILING_MIC_MUTE_BTN)
            {
                _room.AudioCeilingMicMuteToggle();
                return;
            }

            // ── Camera power / tracking toggle (joins 96–97) ─────────────────────
            if (joinNum == JoinMap.CAM_POWER_BTN)
            {
                _room.CameraPowerToggle();
                return;
            }

            if (joinNum == JoinMap.CAM_TRACKING_BTN)
            {
                _room.CameraTrackingToggle();
                return;
            }

            // ── Camera preset recall (joins 98–107) ───────────────────────────────
            if (joinNum >= JoinMap.CAM_PRESET_1 && joinNum <= JoinMap.CAM_PRESET_10)
            {
                _room.CameraPresetRecall(joinNum);
                return;
            }

            // ── Camera select (joins 108–117) ─────────────────────────────────────
            if (joinNum >= JoinMap.CAM_SELECT_1 && joinNum <= JoinMap.CAM_SELECT_10)
            {
                _room.CameraSelect(joinNum);
                return;
            }

            // ── User settings – reset / BYOD toggles (joins 133–137) ────────────
            if (joinNum == JoinMap.SETTINGS_RESET_BTN)
            {
                _room.SettingsReset();
                return;
            }

            if (joinNum == JoinMap.SETTINGS_BYOD_AUTO_SWITCH_BTN)
            {
                _room.SettingsByodAutoSwitchToggle();
                return;
            }

            if (joinNum == JoinMap.SETTINGS_BYOD_AUTO_POWER_BTN)
            {
                _room.SettingsByodAutoPowerToggle();
                return;
            }

            // ── Unhandled join ───────────────────────────────────────────────────
            // ── Master volume default / mute (joins 130–131) ─────────────────────
            if (joinNum == JoinMap.MASTER_VOL_DEFAULT_BTN)
            {
                _room.MasterVolDefault();
                return;
            }

            if (joinNum == JoinMap.MASTER_VOL_MUTE_BTN)
            {
                _room.MasterVolMuteToggle();
                return;
            }

            // ── Unhandled join ───────────────────────────────────────────────────
            CrestronConsole.PrintLine("[ControlSystem] Unhandled digital join {0}", joinNum);
        }

        private void HandleAnalogJoin(Sig sig)
        {
            uint joinNum = sig.Number;

            // ── Audio source volume-set (joins 5–9) ──────────────────────────────
            if (joinNum >= JoinMap.AUDIO_SRC_VOL_SET_1 && joinNum <= JoinMap.AUDIO_SRC_VOL_SET_5)
            {
                _room.AudioVolSet(joinNum, sig.UShortValue);
                return;
            }

            switch (joinNum)
            {
                case JoinMap.BRIGHTNESS_SET:
                    _room.SetBrightness(sig.UShortValue);
                    break;
                case JoinMap.VOLUME_SET:
                    _room.SetVolume(sig.UShortValue);
                    break;
                case JoinMap.SETTINGS_STARTUP_VOL_SET:
                    _room.SettingsStartupVolume(sig.UShortValue);
                    break;
                case JoinMap.CAM_ZOOM_SPEED_SET:
                    _room.CameraZoomSpeed(sig.UShortValue);
                    break;
                case JoinMap.CAM_MOMENT_SPEED_SET:
                    _room.CameraMomentSpeed(sig.UShortValue);
                    break;

                default:
                    CrestronConsole.PrintLine("[ControlSystem] Unhandled analog join {0}", joinNum);
                    break;
            }
        }

        private void HandleSerialJoin(Sig sig)
        {
            uint   joinNum = sig.Number;
            string value   = sig.StringValue ?? string.Empty;

            switch (joinNum)
            {
                case JoinMap.SETTINGS_THEME_MODE:
                    _room.SettingsThemeMode(value);
                    break;
                case JoinMap.SETTINGS_BRAND_COLOR:
                    _room.SettingsBrandColor(value);
                    break;
                case JoinMap.SETTINGS_CLOCK_FORMAT:
                    _room.SettingsClockFormat(value);
                    break;
                case JoinMap.SETTINGS_TEMP_UNIT:
                    _room.SettingsTempUnit(value);
                    break;
                default:
                    CrestronConsole.PrintLine("[ControlSystem] Serial join {0} = \"{1}\"", joinNum, value);
                    break;
            }
        }
    }
}
