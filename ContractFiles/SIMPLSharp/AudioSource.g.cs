using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface IAudioSource
    {
        object UserObject { get; set; }

        /// <summary>Hold high while the user presses the volume-up button.</summary>
        event EventHandler<UIEventArgs> Vol_Up;
        /// <summary>Hold high while the user presses the volume-down button.</summary>
        event EventHandler<UIEventArgs> Vol_Down;
        event EventHandler<UIEventArgs> Default;
        event EventHandler<UIEventArgs> Mute_Toggle;
        /// <summary>Fired when the UI sends an absolute volume level (0–65535).</summary>
        event EventHandler<UIEventArgs> Volume_Set;

        void Mute_Fb(AudioSourceBoolInputSigDelegate callback);
        void Volume_Fb(AudioSourceUShortInputSigDelegate callback);
        void Name_Fb(AudioSourceStringInputSigDelegate callback);
    }

    public delegate void AudioSourceBoolInputSigDelegate(BoolInputSig boolInputSig, IAudioSource audioSource);
    public delegate void AudioSourceUShortInputSigDelegate(UShortInputSig uShortInputSig, IAudioSource audioSource);
    public delegate void AudioSourceStringInputSigDelegate(StringInputSig stringInputSig, IAudioSource audioSource);

    /// <summary>
    /// Audio source – volume up/down hold, default, mute toggle and absolute volume set;
    /// mute feedback, volume level feedback, and source name string.
    /// Reused for AudioSource1 (smartObjectId 19) through AudioSource5 (smartObjectId 23).
    /// </summary>
    internal class AudioSource : IAudioSource, IDisposable
    {
        #region Standard CH5 Component members

        private ComponentMediator ComponentMediator { get; set; }
        public object UserObject { get; set; }
        public uint ControlJoinId { get; private set; }
        private IList<BasicTriListWithSmartObject> _devices;
        public IList<BasicTriListWithSmartObject> Devices { get { return _devices; } }

        #endregion

        #region Joins

        private static class Joins
        {
            internal static class Booleans
            {
                // Events (UI → CS)
                public const uint Vol_Up      = 1;
                public const uint Vol_Down    = 2;
                public const uint Default     = 3;
                public const uint Mute_Toggle = 4;

                // States (CS → UI)
                public const uint Mute_Fb     = 1;
            }
            internal static class Numerics
            {
                // Events (UI → CS)
                public const uint Volume_Set = 1;

                // States (CS → UI)
                public const uint Volume_Fb  = 1;
            }
            internal static class Strings
            {
                // States (CS → UI)
                public const uint Name_Fb = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal AudioSource(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Vol_Up,      onVol_Up);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Vol_Down,    onVol_Down);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Default,     onDefault);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Mute_Toggle, onMute_Toggle);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Volume_Set,  onVolume_Set);
        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        #endregion

        #region CH5 Contract

        public event EventHandler<UIEventArgs> Vol_Up;
        private void onVol_Up(SmartObjectEventArgs e) { var h = Vol_Up; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Vol_Down;
        private void onVol_Down(SmartObjectEventArgs e) { var h = Vol_Down; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Default;
        private void onDefault(SmartObjectEventArgs e) { var h = Default; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Mute_Toggle;
        private void onMute_Toggle(SmartObjectEventArgs e) { var h = Mute_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Volume_Set;
        private void onVolume_Set(SmartObjectEventArgs e) { var h = Volume_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Mute_Fb(AudioSourceBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Mute_Fb], this);
        }

        public void Volume_Fb(AudioSourceUShortInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Volume_Fb], this);
        }

        public void Name_Fb(AudioSourceStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Name_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "AudioSource", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Vol_Up      = null;
            Vol_Down    = null;
            Default     = null;
            Mute_Toggle = null;
            Volume_Set  = null;
        }

        #endregion
    }
}
