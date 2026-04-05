using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;

namespace ExecutiveControlSystem
{
    /// <summary>
    /// Encapsulates all audio control logic for up to five audio sources and
    /// three far-end microphone mutes (Privacy, Wireless Mic, Ceiling Mic).
    ///
    /// <para>
    /// Each source supports:
    /// <list type="bullet">
    ///   <item>Volume Up / Down (hold buttons – bumps level on every press event)</item>
    ///   <item>Default Volume (restores the source to 50 %)</item>
    ///   <item>Mute toggle with feedback</item>
    ///   <item>Absolute volume set via analog join with feedback</item>
    /// </list>
    /// </para>
    ///
    /// <para>Source display names are read from the <c>AudioSources</c> section of
    /// <c>SystemConfig.json</c> and pushed to serial joins 29–33.</para>
    /// </summary>
    internal class Audio
    {
        private const int    MAX_SOURCES    = 5;
        private const ushort DEFAULT_VOLUME = 32767; // 50 % of 65 535
        private const ushort VOLUME_STEP    = 3276;  // ~5 % increment per press

        private readonly BasicTriListWithSmartObject _panel;
        private readonly SystemInfo _systemInfo;

        private readonly ushort[] _volumes = new ushort[MAX_SOURCES];
        private readonly bool[]   _muted   = new bool[MAX_SOURCES];

        private bool _privacyMuted;
        private bool _wirelessMicMuted;
        private bool _ceilingMicMuted;

        // ── Join arrays – indexed 0 = source 1, 4 = source 5 ─────────────────────
        private static readonly uint[] VolUpJoins   = { JoinMap.AUDIO_SRC_VOL_UP_1,   JoinMap.AUDIO_SRC_VOL_UP_2,   JoinMap.AUDIO_SRC_VOL_UP_3,   JoinMap.AUDIO_SRC_VOL_UP_4,   JoinMap.AUDIO_SRC_VOL_UP_5   };
        private static readonly uint[] VolDownJoins = { JoinMap.AUDIO_SRC_VOL_DOWN_1, JoinMap.AUDIO_SRC_VOL_DOWN_2, JoinMap.AUDIO_SRC_VOL_DOWN_3, JoinMap.AUDIO_SRC_VOL_DOWN_4, JoinMap.AUDIO_SRC_VOL_DOWN_5 };
        private static readonly uint[] DefaultJoins = { JoinMap.AUDIO_SRC_DEFAULT_1,  JoinMap.AUDIO_SRC_DEFAULT_2,  JoinMap.AUDIO_SRC_DEFAULT_3,  JoinMap.AUDIO_SRC_DEFAULT_4,  JoinMap.AUDIO_SRC_DEFAULT_5  };
        private static readonly uint[] MuteBtnJoins = { JoinMap.AUDIO_SRC_MUTE_BTN_1, JoinMap.AUDIO_SRC_MUTE_BTN_2, JoinMap.AUDIO_SRC_MUTE_BTN_3, JoinMap.AUDIO_SRC_MUTE_BTN_4, JoinMap.AUDIO_SRC_MUTE_BTN_5 };
        private static readonly uint[] MuteFbJoins  = { JoinMap.AUDIO_SRC_MUTE_FB_1,  JoinMap.AUDIO_SRC_MUTE_FB_2,  JoinMap.AUDIO_SRC_MUTE_FB_3,  JoinMap.AUDIO_SRC_MUTE_FB_4,  JoinMap.AUDIO_SRC_MUTE_FB_5  };
        private static readonly uint[] VolSetJoins  = { JoinMap.AUDIO_SRC_VOL_SET_1,  JoinMap.AUDIO_SRC_VOL_SET_2,  JoinMap.AUDIO_SRC_VOL_SET_3,  JoinMap.AUDIO_SRC_VOL_SET_4,  JoinMap.AUDIO_SRC_VOL_SET_5  };
        private static readonly uint[] VolFbJoins   = { JoinMap.AUDIO_SRC_VOL_FB_1,   JoinMap.AUDIO_SRC_VOL_FB_2,   JoinMap.AUDIO_SRC_VOL_FB_3,   JoinMap.AUDIO_SRC_VOL_FB_4,   JoinMap.AUDIO_SRC_VOL_FB_5   };
        private static readonly uint[] NameJoins    = { JoinMap.AUDIO_SRC_NAME_1,      JoinMap.AUDIO_SRC_NAME_2,      JoinMap.AUDIO_SRC_NAME_3,      JoinMap.AUDIO_SRC_NAME_4,      JoinMap.AUDIO_SRC_NAME_5      };

        /// <param name="panel">The registered touch-panel device.</param>
        /// <param name="systemInfo">Provides access to the loaded JSON configuration.</param>
        public Audio(BasicTriListWithSmartObject panel, SystemInfo systemInfo)
        {
            _panel      = panel      ?? throw new ArgumentNullException(nameof(panel));
            _systemInfo = systemInfo ?? throw new ArgumentNullException(nameof(systemInfo));

            for (int i = 0; i < MAX_SOURCES; i++)
                _volumes[i] = DEFAULT_VOLUME;
        }

        // ── Volume Up (hold) ─────────────────────────────────────────────────────

        /// <summary>
        /// Called on every press/release event for a volume-up join.
        /// Bumps the source volume up by one step on the rising edge.
        /// </summary>
        public void HandleVolUpPress(uint joinNum, bool isPressed)
        {
            if (!isPressed) return;
            int idx = IndexOf(VolUpJoins, joinNum);
            if (idx < 0) return;
            ushort newVol = (ushort)Math.Min(_volumes[idx] + VOLUME_STEP, 65535);
            CrestronConsole.PrintLine("[Audio] Source {0} vol-up → {1}", idx + 1, newVol);
            SetVolume(idx, newVol);
        }

        // ── Volume Down (hold) ───────────────────────────────────────────────────

        /// <summary>
        /// Called on every press/release event for a volume-down join.
        /// Bumps the source volume down by one step on the rising edge.
        /// </summary>
        public void HandleVolDownPress(uint joinNum, bool isPressed)
        {
            if (!isPressed) return;
            int idx = IndexOf(VolDownJoins, joinNum);
            if (idx < 0) return;
            ushort newVol = (ushort)Math.Max(_volumes[idx] - VOLUME_STEP, 0);
            CrestronConsole.PrintLine("[Audio] Source {0} vol-down → {1}", idx + 1, newVol);
            SetVolume(idx, newVol);
        }

        // ── Default Volume ───────────────────────────────────────────────────────

        /// <summary>Restores the source identified by <paramref name="joinNum"/> to the default volume level.</summary>
        public void HandleDefault(uint joinNum)
        {
            int idx = IndexOf(DefaultJoins, joinNum);
            if (idx < 0) return;
            CrestronConsole.PrintLine("[Audio] Source {0} reset to default volume", idx + 1);
            SetVolume(idx, DEFAULT_VOLUME);
        }

        // ── Mute Toggle ──────────────────────────────────────────────────────────

        /// <summary>Toggles the mute state for the source identified by <paramref name="joinNum"/>.</summary>
        public void HandleMuteToggle(uint joinNum)
        {
            int idx = IndexOf(MuteBtnJoins, joinNum);
            if (idx < 0) return;
            _muted[idx] = !_muted[idx];
            CrestronConsole.PrintLine("[Audio] Source {0} mute = {1}", idx + 1, _muted[idx]);
            _panel.BooleanInput[MuteFbJoins[idx]].BoolValue = _muted[idx];
        }

        // ── Analog Volume Set ────────────────────────────────────────────────────

        /// <summary>Sets the volume level for the source whose vol-set join matches <paramref name="joinNum"/>.</summary>
        public void HandleVolSet(uint joinNum, ushort value)
        {
            int idx = IndexOf(VolSetJoins, joinNum);
            if (idx < 0) return;
            CrestronConsole.PrintLine("[Audio] Source {0} vol set to {1}", idx + 1, value);
            SetVolume(idx, value);
        }

        // ── Far-end Audio Mutes ──────────────────────────────────────────────────

        /// <summary>Toggles the privacy-mute state and sends feedback to the panel.</summary>
        public void HandlePrivacyMuteToggle()
        {
            _privacyMuted = !_privacyMuted;
            CrestronConsole.PrintLine("[Audio] Privacy mute = {0}", _privacyMuted);
            _panel.BooleanInput[JoinMap.AUDIO_PRIVACY_MUTE_FB].BoolValue = _privacyMuted;
        }

        /// <summary>Toggles the wireless-microphone mute state and sends feedback to the panel.</summary>
        public void HandleWirelessMicMuteToggle()
        {
            _wirelessMicMuted = !_wirelessMicMuted;
            CrestronConsole.PrintLine("[Audio] Wireless mic mute = {0}", _wirelessMicMuted);
            _panel.BooleanInput[JoinMap.AUDIO_WIRELESS_MIC_MUTE_FB].BoolValue = _wirelessMicMuted;
        }

        /// <summary>Toggles the ceiling-microphone mute state and sends feedback to the panel.</summary>
        public void HandleCeilingMicMuteToggle()
        {
            _ceilingMicMuted = !_ceilingMicMuted;
            CrestronConsole.PrintLine("[Audio] Ceiling mic mute = {0}", _ceilingMicMuted);
            _panel.BooleanInput[JoinMap.AUDIO_CEILING_MIC_MUTE_FB].BoolValue = _ceilingMicMuted;
        }

        // ── Refresh / Initial Push ───────────────────────────────────────────────

        /// <summary>
        /// Pushes the current audio state to the panel.
        /// Call after the panel comes online or when config is (re)loaded.
        /// </summary>
        public void RefreshAll()
        {
            for (int i = 0; i < MAX_SOURCES; i++)
            {
                _panel.UShortInput[VolFbJoins[i]].UShortValue = _volumes[i];
                _panel.BooleanInput[MuteFbJoins[i]].BoolValue  = _muted[i];
            }
            _panel.BooleanInput[JoinMap.AUDIO_PRIVACY_MUTE_FB].BoolValue      = _privacyMuted;
            _panel.BooleanInput[JoinMap.AUDIO_WIRELESS_MIC_MUTE_FB].BoolValue = _wirelessMicMuted;
            _panel.BooleanInput[JoinMap.AUDIO_CEILING_MIC_MUTE_FB].BoolValue  = _ceilingMicMuted;
            SendAudioSourceNames();
        }

        /// <summary>
        /// Reads the <c>AudioSources</c> list from the loaded JSON configuration and
        /// sends each source name to its corresponding serial join (29–33).
        /// </summary>
        public void SendAudioSourceNames()
        {
            if (_systemInfo.Config?.AudioSources == null)
            {
                CrestronConsole.PrintLine("[Audio] Cannot send source names – config not loaded");
                return;
            }

            int count = Math.Min(_systemInfo.Config.AudioSources.Count, MAX_SOURCES);
            for (int i = 0; i < MAX_SOURCES; i++)
            {
                string name = i < count ? (_systemInfo.Config.AudioSources[i].Name ?? string.Empty) : string.Empty;
                _panel.StringInput[NameJoins[i]].StringValue = name;
                if (i < count)
                    CrestronConsole.PrintLine("[Audio] Source {0} name = \"{1}\"", i + 1, name);
            }
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private void SetVolume(int idx, ushort value)
        {
            _volumes[idx] = value;
            _panel.UShortInput[VolFbJoins[idx]].UShortValue = value;
        }

        private static int IndexOf(uint[] joins, uint joinNum)
        {
            for (int i = 0; i < joins.Length; i++)
                if (joins[i] == joinNum) return i;
            return -1;
        }
    }
}
