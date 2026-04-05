using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface ILighting
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Lights_On;
        event EventHandler<UIEventArgs> Lights_Off;
        event EventHandler<UIEventArgs> Lights_Toggle;
        event EventHandler<UIEventArgs> Brightness_Set;

        void Lights_On_Fb(LightingBoolInputSigDelegate callback);
        void Brightness_Fb(LightingUShortInputSigDelegate callback);
    }

    public delegate void LightingBoolInputSigDelegate(BoolInputSig boolInputSig, ILighting lighting);
    public delegate void LightingUShortInputSigDelegate(UShortInputSig uShortInputSig, ILighting lighting);

    /// <summary>
    /// Room lighting on/off/toggle and brightness level.
    /// SmartObjectId: 2
    /// </summary>
    internal class Lighting : ILighting, IDisposable
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
                public const uint Lights_On     = 1;
                public const uint Lights_Off    = 2;
                public const uint Lights_Toggle = 3;

                // States (CS → UI)
                public const uint Lights_On_Fb  = 1;
            }
            internal static class Numerics
            {
                // Events (UI → CS)
                public const uint Brightness_Set = 1;

                // States (CS → UI)
                public const uint Brightness_Fb  = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Lighting(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Lights_On,     onLights_On);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Lights_Off,    onLights_Off);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Lights_Toggle, onLights_Toggle);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Brightness_Set, onBrightness_Set);
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

        public event EventHandler<UIEventArgs> Lights_On;
        private void onLights_On(SmartObjectEventArgs e) { var h = Lights_On; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Lights_Off;
        private void onLights_Off(SmartObjectEventArgs e) { var h = Lights_Off; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Lights_Toggle;
        private void onLights_Toggle(SmartObjectEventArgs e) { var h = Lights_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Brightness_Set;
        private void onBrightness_Set(SmartObjectEventArgs e) { var h = Brightness_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Lights_On_Fb(LightingBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Lights_On_Fb], this);
        }

        public void Brightness_Fb(LightingUShortInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Brightness_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "Lighting", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Lights_On     = null;
            Lights_Off    = null;
            Lights_Toggle = null;
            Brightness_Set = null;
        }

        #endregion
    }
}
