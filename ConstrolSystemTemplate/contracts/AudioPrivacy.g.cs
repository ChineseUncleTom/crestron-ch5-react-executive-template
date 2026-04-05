using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface IAudioPrivacy
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Privacy_Mute_Toggle;
        event EventHandler<UIEventArgs> Wireless_Mic_Mute_Toggle;
        event EventHandler<UIEventArgs> Ceiling_Mic_Mute_Toggle;

        void Privacy_Mute_Fb(AudioPrivacyBoolInputSigDelegate callback);
        void Wireless_Mic_Mute_Fb(AudioPrivacyBoolInputSigDelegate callback);
        void Ceiling_Mic_Mute_Fb(AudioPrivacyBoolInputSigDelegate callback);
    }

    public delegate void AudioPrivacyBoolInputSigDelegate(BoolInputSig boolInputSig, IAudioPrivacy audioPrivacy);

    /// <summary>
    /// Privacy, wireless, and ceiling microphone mutes.
    /// SmartObjectId: 4
    /// </summary>
    internal class AudioPrivacy : IAudioPrivacy, IDisposable
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
                public const uint Privacy_Mute_Toggle       = 1;
                public const uint Wireless_Mic_Mute_Toggle  = 2;
                public const uint Ceiling_Mic_Mute_Toggle   = 3;

                // States (CS → UI)
                public const uint Privacy_Mute_Fb           = 1;
                public const uint Wireless_Mic_Mute_Fb      = 2;
                public const uint Ceiling_Mic_Mute_Fb       = 3;
            }
        }

        #endregion

        #region Construction and Initialization

        internal AudioPrivacy(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Privacy_Mute_Toggle,      onPrivacy_Mute_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Wireless_Mic_Mute_Toggle, onWireless_Mic_Mute_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Ceiling_Mic_Mute_Toggle,  onCeiling_Mic_Mute_Toggle);
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

        public event EventHandler<UIEventArgs> Privacy_Mute_Toggle;
        private void onPrivacy_Mute_Toggle(SmartObjectEventArgs e) { var h = Privacy_Mute_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Wireless_Mic_Mute_Toggle;
        private void onWireless_Mic_Mute_Toggle(SmartObjectEventArgs e) { var h = Wireless_Mic_Mute_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Ceiling_Mic_Mute_Toggle;
        private void onCeiling_Mic_Mute_Toggle(SmartObjectEventArgs e) { var h = Ceiling_Mic_Mute_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Privacy_Mute_Fb(AudioPrivacyBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Privacy_Mute_Fb], this);
        }

        public void Wireless_Mic_Mute_Fb(AudioPrivacyBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Wireless_Mic_Mute_Fb], this);
        }

        public void Ceiling_Mic_Mute_Fb(AudioPrivacyBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Ceiling_Mic_Mute_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "AudioPrivacy", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Privacy_Mute_Toggle      = null;
            Wireless_Mic_Mute_Toggle = null;
            Ceiling_Mic_Mute_Toggle  = null;
        }

        #endregion
    }
}
