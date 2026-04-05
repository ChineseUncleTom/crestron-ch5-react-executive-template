using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface ISystemControl
    {
        object UserObject { get; set; }

        /// <summary>Fired when the UI requests room startup.</summary>
        event EventHandler<UIEventArgs> System_Startup;
        /// <summary>Fired when the UI requests room shutdown.</summary>
        event EventHandler<UIEventArgs> System_Off;
        /// <summary>Fired when the UI requests "present local PC to all displays".</summary>
        event EventHandler<UIEventArgs> Present_To_Room;
        /// <summary>Fired when the UI toggles Teams conferencing mode.</summary>
        event EventHandler<UIEventArgs> Teams_Mode_Toggle;
        /// <summary>Fired when the UI toggles BYOD mode.</summary>
        event EventHandler<UIEventArgs> BYOD_Mode_Toggle;
        /// <summary>Fired when the UI requests BYOD device connection.</summary>
        event EventHandler<UIEventArgs> BYOD_Select;

        void System_On_Fb(SystemControlBoolInputSigDelegate callback);
        void Teams_Mode_Fb(SystemControlBoolInputSigDelegate callback);
        void BYOD_Mode_Fb(SystemControlBoolInputSigDelegate callback);
    }

    public delegate void SystemControlBoolInputSigDelegate(BoolInputSig boolInputSig, ISystemControl systemControl);

    /// <summary>
    /// Room power, scenario macros and conferencing mode flags.
    /// SmartObjectId: 1
    /// </summary>
    internal class SystemControl : ISystemControl, IDisposable
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
                public const uint System_Startup    = 1;
                public const uint System_Off        = 2;
                public const uint Present_To_Room   = 3;
                public const uint Teams_Mode_Toggle = 4;
                public const uint BYOD_Mode_Toggle  = 5;
                public const uint BYOD_Select       = 6;

                // States (CS → UI)
                public const uint System_On_Fb      = 1;
                public const uint Teams_Mode_Fb     = 2;
                public const uint BYOD_Mode_Fb      = 3;
            }
        }

        #endregion

        #region Construction and Initialization

        internal SystemControl(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.System_Startup,    onSystem_Startup);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.System_Off,        onSystem_Off);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Present_To_Room,   onPresent_To_Room);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Teams_Mode_Toggle, onTeams_Mode_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.BYOD_Mode_Toggle,  onBYOD_Mode_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.BYOD_Select,       onBYOD_Select);
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

        public event EventHandler<UIEventArgs> System_Startup;
        private void onSystem_Startup(SmartObjectEventArgs e) { var h = System_Startup; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> System_Off;
        private void onSystem_Off(SmartObjectEventArgs e) { var h = System_Off; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Present_To_Room;
        private void onPresent_To_Room(SmartObjectEventArgs e) { var h = Present_To_Room; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Teams_Mode_Toggle;
        private void onTeams_Mode_Toggle(SmartObjectEventArgs e) { var h = Teams_Mode_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> BYOD_Mode_Toggle;
        private void onBYOD_Mode_Toggle(SmartObjectEventArgs e) { var h = BYOD_Mode_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> BYOD_Select;
        private void onBYOD_Select(SmartObjectEventArgs e) { var h = BYOD_Select; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void System_On_Fb(SystemControlBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.System_On_Fb], this);
        }

        public void Teams_Mode_Fb(SystemControlBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Teams_Mode_Fb], this);
        }

        public void BYOD_Mode_Fb(SystemControlBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.BYOD_Mode_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "SystemControl", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            System_Startup    = null;
            System_Off        = null;
            Present_To_Room   = null;
            Teams_Mode_Toggle = null;
            BYOD_Mode_Toggle  = null;
            BYOD_Select       = null;
        }

        #endregion
    }
}
