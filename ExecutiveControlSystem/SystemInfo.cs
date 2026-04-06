using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using System.Text.Json;

namespace ExecutiveControlSystem
{
    /// <summary>
    /// Provides system information services: date/time updates and JSON
    /// configuration loading from <c>Nvram/SystemConfig.json</c>.
    /// </summary>
    internal class SystemInfo
    {
        private readonly BasicTriListWithSmartObject _panel;
        private CTimer _dateTimeTimer;
        private RoomConfig _config;
        private string _clockFormat = "12h";

        /// <summary>Gets the loaded room configuration.</summary>
        public RoomConfig Config => _config;

        /// <summary>
        /// Gets or sets the clock format used when sending the time string to the panel.
        /// Accepted values are <c>"12h"</c> (default) and <c>"24h"</c>.
        /// </summary>
        public string ClockFormat
        {
            get => _clockFormat;
            set => _clockFormat = value ?? "12h";
        }

        private const string ConfigFileName = "SystemConfig.json";

        /// <summary>
        /// Initializes a new instance of the SystemInfo class.
        /// </summary>
        /// <param name="panel">The panel device to send system information to.</param>
        public SystemInfo(BasicTriListWithSmartObject panel)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));
        }

        #region Data Models for JSON Configuration

        public class RoomConfig
        {
            public RoomInfo RoomInfo { get; set; }
            public List<Room> Rooms { get; set; }
            public List<Wall> Walls { get; set; }
            public List<Camera> Cameras { get; set; }
            public List<CameraSwitcher> CameraSwitchers { get; set; }
            public List<ProjectorScreen> ProjectorScreens { get; set; }
            public List<DspDevice> Dsp { get; set; }
            public List<CeilingMic> CeilingMics { get; set; }
            public List<Wap> Wap { get; set; }
            public List<VideoSource> VideoSources { get; set; }
            public List<VideoDestination> VideoDestinations { get; set; }
            public List<int> VideoSourceLocates { get; set; }
            public List<AudioSource> AudioSources { get; set; }
            public List<LightwareDevice> Lightware { get; set; }
            public List<IpDevice> Displays { get; set; }
            public List<IpDevice> Projectors { get; set; }
            public List<RoomVideoConfig> RoomVideoConfig { get; set; }
            public HelpSupportInfo HelpSupport { get; set; }
        }

        public class RoomInfo
        {
            public string Name { get; set; }
            public string Number { get; set; }
            public int PrimaryRoomNumber { get; set; }
        }

        public class Room
        {
            public string Name { get; set; }
            public string Number { get; set; }
            public int Type { get; set; }
        }

        public class Wall
        {
            public string Name { get; set; }
        }

        public class CameraPreset
        {
            public string Name { get; set; }
            public int Index { get; set; }
        }

        public class Camera
        {
            public string Name { get; set; }
            public string IpAddress { get; set; }
            public string PreviewUrl { get; set; }
            public int Location { get; set; }
            public int SwitcherInput { get; set; }
            public List<CameraPreset> Presets { get; set; }
        }

        public class CameraSwitcher
        {
            public string Room { get; set; }
            public string IpAddress { get; set; }
        }

        public class ProjectorScreen
        {
            public string Name { get; set; }
            public int Room { get; set; }
            public int Relay { get; set; }
        }

        public class DspDevice
        {
            public string Room { get; set; }
            public string IpAddress { get; set; }
        }

        public class CeilingMic
        {
            public string Room { get; set; }
            public int Index { get; set; }
            public string IpAddress { get; set; }
        }

        public class Wap
        {
            public int Index { get; set; }
            public string MicNumber { get; set; }
            public string IpAddress { get; set; }
            public List<string> MicNames { get; set; }
        }

        public class VideoSource
        {
            public string Name { get; set; }
            public uint Id { get; internal set; }
            /// <summary>When true, this source originates from the Room PC (e.g. Teams content window).
            /// Used by Ingest Mode to separate Room PC sources from shareable external sources.</summary>
            public bool IsRoomPC { get; set; }
        }

        public class VideoDestination
        {
            public string Name { get; set; }
            public bool IsDisplay { get; set; }
            public bool IsLocal { get; set; }
            public int DmLocate { get; set; }
        }

        public class AudioSource
        {
            public string Name { get; set; }
            public uint Id { get; internal set; }
        }

        public class LightwareDevice
        {
            public string Room { get; set; }
            public string IpAddress { get; set; }
        }

        public class IpDevice
        {
            public int Index { get; set; }
            public string IpAddress { get; set; }
        }

        public class RoomVideoConfig
        {
            public string Room { get; set; }
            public bool VideoSwitchSourceOnDm { get; set; }
            public bool TeamsSecondaryDisplay { get; set; }
            public int DmOutToTeams { get; set; }
            public List<RoomDestination> Destinations { get; set; }
        }

        public class RoomDestination
        {
            public int DestIndex { get; set; }
            public bool IsLocal { get; set; }
        }

        public class HelpSupportInfo
        {
            public string ItHelpdesk  { get; set; }
            public string SupportEmail { get; set; }
            public string QrLabel      { get; set; }
        }

        #endregion

        // ── Date / Time ───────────────────────────────────────────────────────────

        /// <summary>Starts updating date and time to the panel every second.</summary>
        public void StartDateTimeUpdates()
        {
            UpdateDateTime();
            _dateTimeTimer = new CTimer(DateTimeTimerCallback, null, 1000, 1000);
            CrestronConsole.PrintLine("[SystemInfo] Date/Time updates started");
        }

        /// <summary>Stops the date/time updates.</summary>
        public void StopDateTimeUpdates()
        {
            if (_dateTimeTimer != null)
            {
                _dateTimeTimer.Stop();
                _dateTimeTimer.Dispose();
                _dateTimeTimer = null;
                CrestronConsole.PrintLine("[SystemInfo] Date/Time updates stopped");
            }
        }

        private void DateTimeTimerCallback(object userSpecific)
        {
            UpdateDateTime();
        }

        /// <summary>Updates the date and time strings on the panel.</summary>
        public void UpdateDateTime()
        {
            try
            {
                DateTime now = DateTime.Now;
                string timeFormat = _clockFormat == "24h" ? "HH:mm" : "hh:mm tt";
                _panel.StringInput[JoinMap.DATE_SERIAL].StringValue = now.ToString("dddd, MMMM dd, yyyy");
                _panel.StringInput[JoinMap.TIME_SERIAL].StringValue = now.ToString(timeFormat);
            }
            catch (Exception ex)
            {
                ErrorLog.Error("[SystemInfo] Error updating date/time: {0}", ex.Message);
            }
        }

        // ── JSON Config ───────────────────────────────────────────────────────────

        /// <summary>
        /// Loads <c>SystemConfig.json</c> from the Nvram directory, deserializes it,
        /// and pushes room name / room number to the panel serial joins.
        /// </summary>
        public void LoadSystemConfig()
        {
            string configFilePath = Path.Combine(
                Directory.GetApplicationRootDirectory(),
                $"Nvram/{ConfigFileName}");

            if (!File.Exists(configFilePath))
            {
                CrestronConsole.PrintLine("[SystemInfo] Configuration file NOT found at: {0}", configFilePath);
                return;
            }

            CrestronConsole.PrintLine("[SystemInfo] Configuration file found: {0}", configFilePath);
            ReadJson(configFilePath);

            if (_config?.RoomInfo != null)
            {
                _panel.StringInput[JoinMap.ROOM_NAME_SERIAL].StringValue   = _config.RoomInfo.Name   ?? string.Empty;
                _panel.StringInput[JoinMap.ROOM_NUMBER_SERIAL].StringValue = _config.RoomInfo.Number ?? string.Empty;
                CrestronConsole.PrintLine("[SystemInfo] Room: {0} ({1})", _config.RoomInfo.Name, _config.RoomInfo.Number);
            }
            else
            {
                CrestronConsole.PrintLine("[SystemInfo] Config loaded but RoomInfo is null");
            }

            if (_config?.HelpSupport != null)
            {
                _panel.StringInput[JoinMap.HELP_IT_PHONE_SERIAL].StringValue      = _config.HelpSupport.ItHelpdesk   ?? string.Empty;
                _panel.StringInput[JoinMap.HELP_SUPPORT_EMAIL_SERIAL].StringValue = _config.HelpSupport.SupportEmail ?? string.Empty;
                _panel.StringInput[JoinMap.HELP_QR_LABEL_SERIAL].StringValue      = _config.HelpSupport.QrLabel      ?? string.Empty;
                CrestronConsole.PrintLine("[SystemInfo] Help & Support info pushed to panel");
            }
        }

        // ── Network Info ──────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the control system's LAN adapter parameters (IP address, subnet mask,
        /// MAC address) using <see cref="CrestronEthernetHelper"/> and pushes them to
        /// the corresponding panel serial joins.
        /// <para>
        /// Both the "Panel IP" and "Control System IP" fields are populated with the
        /// same LAN-A adapter address because, in the XPanel/H5 architecture used by
        /// this template, the web panel is served directly by the control system processor.
        /// </para>
        /// </summary>
        public void PushNetworkInfo()
        {
            try
            {
                string ip     = CrestronEthernetHelper.GetEthernetParameter(
                                    CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS,
                                    0);
                string subnet = CrestronEthernetHelper.GetEthernetParameter(
                                    CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_MASK,
                                    0);
                string mac    = CrestronEthernetHelper.GetEthernetParameter(
                                    CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_MAC_ADDRESS,
                                    0);

                _panel.StringInput[JoinMap.NETWORK_PANEL_IP_SERIAL].StringValue = ip     ?? string.Empty;
                _panel.StringInput[JoinMap.NETWORK_SUBNET_SERIAL].StringValue   = subnet ?? string.Empty;
                _panel.StringInput[JoinMap.NETWORK_MAC_SERIAL].StringValue      = mac    ?? string.Empty;
                _panel.StringInput[JoinMap.NETWORK_CS_IP_SERIAL].StringValue    = ip     ?? string.Empty;

                CrestronConsole.PrintLine("[SystemInfo] Network info pushed – IP: {0}  Subnet: {1}  MAC: {2}", ip, subnet, mac);
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("[SystemInfo] Error reading network info: {0}", ex.Message);
                ErrorLog.Error("[SystemInfo] Error reading network info: {0}", ex.Message);
            }
        }

        private void ReadJson(string filePath)
        {
            try
            {
                string json;
                using (var reader = new StreamReader(filePath, System.Text.Encoding.Default))
                    json = reader.ReadToEnd();

                CrestronConsole.PrintLine("[SystemInfo] JSON read OK ({0} chars)", json.Length);
                _config = JsonSerializer.Deserialize<RoomConfig>(json);

                if (_config != null)
                    CrestronConsole.PrintLine("[SystemInfo] JSON deserialized OK");
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("[SystemInfo] Error reading JSON: {0}", ex.Message);
                ErrorLog.Error("[SystemInfo] Error reading JSON: {0}", ex.Message);
            }
        }
    }
}
