using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.Keypads;
using System.Text.Json;

namespace ConstrolSystemTemplate
{
    internal class SystemInfo
    {
        private readonly BasicTriListWithSmartObject _panel;
        private CTimer _dateTimeTimer;
        private RoomConfig config;

        /// <summary>Gets the loaded room configuration.</summary>
        public RoomConfig Config => config;

        const string configFileName = "SystemConfig.json";

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
        #endregion
        
        string JSONDataASString = "";

        /// <summary>
        /// Starts updating date and time to the panel every second.
        /// </summary>
        public void StartDateTimeUpdates()
        {
            // Update immediately
            UpdateDateTime();

            // Create a timer that fires every 1000ms (1 second)
            _dateTimeTimer = new CTimer(DateTimeTimerCallback, null, 1000, 1000);
            CrestronConsole.PrintLine("[SystemInfo] Date/Time updates started");
        }

        /// <summary>
        /// Stops the date/time updates.
        /// </summary>
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

        /// <summary>
        /// Timer callback that updates date and time.
        /// </summary>
        private void DateTimeTimerCallback(object userSpecific)
        {
            UpdateDateTime();
        }

        /// <summary>
        /// Updates the date and time on the panel.
        /// </summary>
        public void UpdateDateTime()
        {
            try
            {
                DateTime now = DateTime.Now;

                // Format date (e.g., "Monday, January 15, 2024" or "01/15/2024")
                string dateString = now.ToString("dddd, MMMM dd, yyyy");

                // Format time (e.g., "02:30 PM" for 12-hour or "14:30" for 24-hour)
                string timeString = now.ToString("hh:mm tt");

                // Send to panel
                _panel.StringInput[JoinMap.DATE_SERIAL].StringValue = dateString;
                _panel.StringInput[JoinMap.TIME_SERIAL].StringValue = timeString;
            }
            catch (Exception ex)
            {
                ErrorLog.Error("[SystemInfo] Error updating date/time: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Gets the current date as a formatted string.
        /// </summary>
        /// <returns>Formatted date string.</returns>
        public string GetFormattedDate()
        {
            return DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }

        /// <summary>
        /// Gets the current time as a formatted string.
        /// </summary>
        /// <returns>Formatted time string.</returns>
        public string GetFormattedTime()
        {
            return DateTime.Now.ToString("hh:mm tt");
        }

        // TODO: read room name, room number, and other details from a json file in the root of the project

        // Load configuration from a JSON file
        public void LoadSystemConfig()
        { 
            string configFilePath = Path.Combine(Directory.GetApplicationRootDirectory(), $"Nvram/{configFileName}");

            if (File.Exists(configFilePath))
            {
                CrestronConsole.PrintLine("[SystemInfo] Configuration file found: {0}", configFilePath);

                // Read and deserialize the JSON file
                ReadJSON(configFilePath);

                // Check if config was successfully loaded
                if (config != null && config.RoomInfo != null)
                {
                    _panel.StringInput[JoinMap.ROOM_NAME_SERIAL].StringValue = config.RoomInfo.Name ?? string.Empty;
                    _panel.StringInput[JoinMap.ROOM_NUMBER_SERIAL].StringValue = config.RoomInfo.Number ?? string.Empty;

                    CrestronConsole.PrintLine("[SystemInfo] Room Name: {0}", config.RoomInfo.Name);
                    CrestronConsole.PrintLine("[SystemInfo] Room Number: {0}", config.RoomInfo.Number);
                }
                else
                {
                    CrestronConsole.PrintLine("[SystemInfo] Configuration loaded but RoomInfo is null");
                }
            }
            else
            { 
                CrestronConsole.PrintLine("[SystemInfo] Configuration file NOT found at: {0}", configFilePath);
            }
        }

        void ReadJSON(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.Default))
                {
                    JSONDataASString = reader.ReadToEnd();
                }

                CrestronConsole.PrintLine("[SystemInfo] JSON file read successfully. Length: {0} characters", JSONDataASString.Length);

                config = JsonSerializer.Deserialize<RoomConfig>(JSONDataASString);

                if (config != null)
                {
                    CrestronConsole.PrintLine("[SystemInfo] JSON deserialized successfully");
                }
            }
            catch (Exception ex)
            { 
                CrestronConsole.PrintLine("[SystemInfo] Error reading JSON file: {0}", ex.Message);
                ErrorLog.Error("[SystemInfo] Error reading JSON file: {0}", ex.Message);
            }
        }
    }
}
