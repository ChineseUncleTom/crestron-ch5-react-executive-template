using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface IMasterVolume
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Mute_Toggle;
        event EventHandler<UIEventArgs> Default;
        event EventHandler<UIEventArgs> Volume_Set;

        void Mute_Fb(MasterVolumeBoolInputSigDelegate callback);
        void Volume_Fb(MasterVolumeUShortInputSigDelegate callback);
    }

    public delegate void MasterVolumeBoolInputSigDelegate(BoolInputSig boolInputSig, IMasterVolume masterVolume);
    public delegate void MasterVolumeUShortInputSigDelegate(UShortInputSig uShortInputSig, IMasterVolume masterVolume);

    /// <summary>
    /// Master output volume level and mute.
    /// SmartObjectId: 3
    /// </summary>
    internal class MasterVolume : IMasterVolume, IDisposable
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
                public const uint Mute_Toggle = 1;
                public const uint Default     = 2;

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
        }

        #endregion

        #region Construction and Initialization

        internal MasterVolume(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Mute_Toggle, onMute_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Default,     onDefault);
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

        public event EventHandler<UIEventArgs> Mute_Toggle;
        private void onMute_Toggle(SmartObjectEventArgs e) { var h = Mute_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Default;
        private void onDefault(SmartObjectEventArgs e) { var h = Default; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Volume_Set;
        private void onVolume_Set(SmartObjectEventArgs e) { var h = Volume_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Mute_Fb(MasterVolumeBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Mute_Fb], this);
        }

        public void Volume_Fb(MasterVolumeUShortInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Volume_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "MasterVolume", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Mute_Toggle = null;
            Default     = null;
            Volume_Set  = null;
        }

        #endregion
    }
}
