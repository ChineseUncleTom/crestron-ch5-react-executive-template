using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Newtonsoft.Json;

namespace ExecutiveControlSystem
{
    /// <summary>
    /// Manages user-configurable settings persisted in <c>/nvram/CurrentUserConfig.json</c>.
    /// Loads settings on startup (preferring <c>CurrentUserConfig.json</c> and falling back to
    /// <c>DefaultUserConfig.json</c>), pushes them to the panel via joins, saves individual
    /// changes received from the panel to <c>CurrentUserConfig.json</c>, and supports
    /// resetting to factory defaults.
    /// </summary>
    internal class UserConfig
    {
        private readonly BasicTriListWithSmartObject _panel;

        private const string DefaultConfigFileName  = "DefaultUserConfig.json";
        private const string CurrentConfigFileName  = "CurrentUserConfig.json";

        // ── Default values ─────────────────────────────────────────────────────

        private const string DefaultThemeMode   = "dark";
        private const string DefaultBrandColor  = "#3b82f6";
        private const bool   DefaultByodAutoSwitch  = true;
        private const bool   DefaultByodAutoPowerOn = false;
        private const int    DefaultStartupVolume   = 30;      // 0–100 percent
        private const string DefaultClockFormat = "12h";
        private const string DefaultTempUnit    = "F";

        // ── Data model ─────────────────────────────────────────────────────────

        public class UserConfigData
        {
            public string ThemeMode      { get; set; } = DefaultThemeMode;
            public string BrandColor     { get; set; } = DefaultBrandColor;
            public bool   ByodAutoSwitch  { get; set; } = DefaultByodAutoSwitch;
            public bool   ByodAutoPowerOn { get; set; } = DefaultByodAutoPowerOn;
            public int    StartupVolume  { get; set; } = DefaultStartupVolume;
            public string ClockFormat    { get; set; } = DefaultClockFormat;
            public string TempUnit       { get; set; } = DefaultTempUnit;
        }

        private UserConfigData _data = new UserConfigData();

        /// <summary>Gets the currently loaded user configuration.</summary>
        public UserConfigData Data => _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserConfig"/> class.
        /// </summary>
        /// <param name="panel">The registered touch-panel device.</param>
        public UserConfig(BasicTriListWithSmartObject panel)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));
        }

        // ── Load / Save ────────────────────────────────────────────────────────

        /// <summary>
        /// Loads settings from <c>/nvram</c> and pushes all values to the panel via their joins.
        /// Priority: <c>CurrentUserConfig.json</c> → <c>DefaultUserConfig.json</c> → hardcoded defaults.
        /// If neither file exists, hardcoded defaults are written to <c>CurrentUserConfig.json</c>.
        /// </summary>
        public void LoadAndPush()
        {
            string currentPath = GetCurrentFilePath();
            string defaultPath = GetDefaultFilePath();

            if (File.Exists(currentPath))
            {
                CrestronConsole.PrintLine("[UserConfig] Loading from CurrentUserConfig.json");
                ReadFile(currentPath);
            }
            else if (File.Exists(defaultPath))
            {
                CrestronConsole.PrintLine("[UserConfig] CurrentUserConfig.json not found – loading from DefaultUserConfig.json");
                ReadFile(defaultPath);
            }
            else
            {
                CrestronConsole.PrintLine("[UserConfig] No config file found – creating defaults at: {0}", currentPath);
                _data = new UserConfigData();
                WriteFile(currentPath);
            }

            PushAllToPanel();
        }

        /// <summary>
        /// Resets all settings to factory defaults, saves <c>CurrentUserConfig.json</c>,
        /// and pushes the new values to the panel.
        /// </summary>
        public void ResetToDefaults()
        {
            CrestronConsole.PrintLine("[UserConfig] Resetting all settings to defaults");
            _data = new UserConfigData();
            WriteFile(GetCurrentFilePath());
            PushAllToPanel();
        }

        /// <summary>Saves the current in-memory settings to <c>CurrentUserConfig.json</c>.</summary>
        public void Save()
        {
            WriteFile(GetCurrentFilePath());
        }

        // ── Receive from panel ─────────────────────────────────────────────────

        /// <summary>Called when the panel sends an updated theme-mode string.</summary>
        public void HandleThemeMode(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            _data.ThemeMode = value;
            Save();
            CrestronConsole.PrintLine("[UserConfig] ThemeMode saved: {0}", value);
        }

        /// <summary>Called when the panel sends an updated brand-colour string.</summary>
        public void HandleBrandColor(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            _data.BrandColor = value;
            Save();
            CrestronConsole.PrintLine("[UserConfig] BrandColor saved: {0}", value);
        }

        /// <summary>Called when the panel sends an updated clock-format string.</summary>
        public void HandleClockFormat(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            _data.ClockFormat = value;
            Save();
            CrestronConsole.PrintLine("[UserConfig] ClockFormat saved: {0}", value);
        }

        /// <summary>Called when the panel sends an updated temperature-unit string.</summary>
        public void HandleTempUnit(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            _data.TempUnit = value;
            Save();
            CrestronConsole.PrintLine("[UserConfig] TempUnit saved: {0}", value);
        }

        /// <summary>Called when the panel sends a new startup volume analog value (0–65535).</summary>
        public void HandleStartupVolume(ushort rawValue)
        {
            // Convert 0–65535 to 0–100 percent for storage
            int pct = (int)Math.Round((rawValue / 65535.0) * 100);
            _data.StartupVolume = pct;
            Save();
            CrestronConsole.PrintLine("[UserConfig] StartupVolume saved: {0}% (raw {1})", pct, rawValue);
        }

        /// <summary>Toggles the BYOD auto-switch setting and saves.</summary>
        public void ToggleByodAutoSwitch()
        {
            _data.ByodAutoSwitch = !_data.ByodAutoSwitch;
            Save();
            CrestronConsole.PrintLine("[UserConfig] ByodAutoSwitch = {0}", _data.ByodAutoSwitch);
            _panel.BooleanInput[JoinMap.SETTINGS_BYOD_AUTO_SWITCH_FB].BoolValue = _data.ByodAutoSwitch;
        }

        /// <summary>Toggles the BYOD auto-power-on setting and saves.</summary>
        public void ToggleByodAutoPowerOn()
        {
            _data.ByodAutoPowerOn = !_data.ByodAutoPowerOn;
            Save();
            CrestronConsole.PrintLine("[UserConfig] ByodAutoPowerOn = {0}", _data.ByodAutoPowerOn);
            _panel.BooleanInput[JoinMap.SETTINGS_BYOD_AUTO_POWER_FB].BoolValue = _data.ByodAutoPowerOn;
        }

        // ── Push to panel ──────────────────────────────────────────────────────

        /// <summary>Sends all current settings values to the panel via their joins.</summary>
        public void PushAllToPanel()
        {
            _panel.StringInput[JoinMap.SETTINGS_THEME_MODE].StringValue   = _data.ThemeMode   ?? DefaultThemeMode;
            _panel.StringInput[JoinMap.SETTINGS_BRAND_COLOR].StringValue  = _data.BrandColor  ?? DefaultBrandColor;
            _panel.StringInput[JoinMap.SETTINGS_CLOCK_FORMAT].StringValue = _data.ClockFormat ?? DefaultClockFormat;
            _panel.StringInput[JoinMap.SETTINGS_TEMP_UNIT].StringValue    = _data.TempUnit    ?? DefaultTempUnit;

            // Convert percent (0–100) to Crestron analog range (0–65535)
            ushort volRaw = (ushort)Math.Round((_data.StartupVolume / 100.0) * 65535);
            _panel.UShortInput[JoinMap.SETTINGS_STARTUP_VOL_FB].UShortValue = volRaw;

            _panel.BooleanInput[JoinMap.SETTINGS_BYOD_AUTO_SWITCH_FB].BoolValue = _data.ByodAutoSwitch;
            _panel.BooleanInput[JoinMap.SETTINGS_BYOD_AUTO_POWER_FB].BoolValue  = _data.ByodAutoPowerOn;

            CrestronConsole.PrintLine("[UserConfig] All settings pushed to panel");
        }

        // ── File helpers ───────────────────────────────────────────────────────

        private static string GetCurrentFilePath()
        {
            string appDir = Crestron.SimplSharp.CrestronIO.Directory.GetApplicationDirectory();
            return Path.Combine(appDir, $"/nvram{CurrentConfigFileName}");
        }

        private static string GetDefaultFilePath()
        {
            string appDir = Crestron.SimplSharp.CrestronIO.Directory.GetApplicationDirectory();
            return Path.Combine(appDir, $"/nvram{DefaultConfigFileName}");
        }

        private void ReadFile(string filePath)
        {
            try
            {
                string json;
                using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
                    json = sr.ReadToEnd();

                CrestronConsole.PrintLine("[UserConfig] JSON read: {0}", json);

                var loaded = JsonConvert.DeserializeObject<UserConfigData>(json);
                if (loaded != null)
                {
                    _data = loaded;
                    CrestronConsole.PrintLine("[UserConfig] Loaded from file OK");
                }
                else
                {
                    CrestronConsole.PrintLine("[UserConfig] Deserialise returned null – using defaults");
                    _data = new UserConfigData();
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("[UserConfig] Read error: {0} – using defaults", ex.Message);
                ErrorLog.Error("[UserConfig] Read error: {0}", ex.Message);
                _data = new UserConfigData();
            }
        }

        private void WriteFile(string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(_data, Formatting.Indented);
                using (FileStream fs = File.Create(filePath))
                    fs.Write(json, System.Text.Encoding.UTF8);

                CrestronConsole.PrintLine("[UserConfig] Saved to file OK");
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("[UserConfig] Write error: {0}", ex.Message);
                ErrorLog.Error("[UserConfig] Write error: {0}", ex.Message);
            }
        }
    }
}
