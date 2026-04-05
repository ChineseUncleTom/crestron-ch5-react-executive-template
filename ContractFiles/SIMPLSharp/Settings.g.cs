using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface ISettings
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Reset;
        event EventHandler<UIEventArgs> BYOD_Auto_Switch_Toggle;
        event EventHandler<UIEventArgs> BYOD_Auto_Power_Toggle;
        event EventHandler<UIEventArgs> Startup_Vol_Set;
        event EventHandler<UIEventArgs> Theme_Mode_Set;
        event EventHandler<UIEventArgs> Brand_Color_Set;
        event EventHandler<UIEventArgs> Clock_Format_Set;
        event EventHandler<UIEventArgs> Temp_Unit_Set;

        void BYOD_Auto_Switch_Fb(SettingsBoolInputSigDelegate callback);
        void BYOD_Auto_Power_Fb(SettingsBoolInputSigDelegate callback);
        void Startup_Vol_Fb(SettingsUShortInputSigDelegate callback);
        void Theme_Mode_Fb(SettingsStringInputSigDelegate callback);
        void Brand_Color_Fb(SettingsStringInputSigDelegate callback);
        void Clock_Format_Fb(SettingsStringInputSigDelegate callback);
        void Temp_Unit_Fb(SettingsStringInputSigDelegate callback);
    }

    public delegate void SettingsBoolInputSigDelegate(BoolInputSig boolInputSig, ISettings settings);
    public delegate void SettingsUShortInputSigDelegate(UShortInputSig uShortInputSig, ISettings settings);
    public delegate void SettingsStringInputSigDelegate(StringInputSig stringInputSig, ISettings settings);

    /// <summary>
    /// User and admin panel settings – theme, brand colour, BYOD behaviour,
    /// audio startup volume, clock format, and temperature unit.
    /// SmartObjectId: 46
    /// </summary>
    internal class Settings : ISettings, IDisposable
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
                public const uint Reset                  = 1;
                public const uint BYOD_Auto_Switch_Toggle = 2;
                public const uint BYOD_Auto_Power_Toggle  = 3;

                // States (CS → UI)
                public const uint BYOD_Auto_Switch_Fb    = 1;
                public const uint BYOD_Auto_Power_Fb     = 2;
            }
            internal static class Numerics
            {
                // Events (UI → CS)
                public const uint Startup_Vol_Set = 1;

                // States (CS → UI)
                public const uint Startup_Vol_Fb  = 1;
            }
            internal static class Strings
            {
                // Events (UI → CS) and States (CS → UI) share the same join numbers
                // because these are bidirectional settings echoed back by the processor.
                public const uint Theme_Mode  = 1;
                public const uint Brand_Color = 2;
                public const uint Clock_Format = 3;
                public const uint Temp_Unit   = 4;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Settings(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Reset,                  onReset);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.BYOD_Auto_Switch_Toggle, onBYOD_Auto_Switch_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.BYOD_Auto_Power_Toggle,  onBYOD_Auto_Power_Toggle);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Startup_Vol_Set,         onStartup_Vol_Set);
            ComponentMediator.ConfigureStringEvent(controlJoinId,  Joins.Strings.Theme_Mode,               onTheme_Mode_Set);
            ComponentMediator.ConfigureStringEvent(controlJoinId,  Joins.Strings.Brand_Color,              onBrand_Color_Set);
            ComponentMediator.ConfigureStringEvent(controlJoinId,  Joins.Strings.Clock_Format,             onClock_Format_Set);
            ComponentMediator.ConfigureStringEvent(controlJoinId,  Joins.Strings.Temp_Unit,                onTemp_Unit_Set);
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

        public event EventHandler<UIEventArgs> Reset;
        private void onReset(SmartObjectEventArgs e) { var h = Reset; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> BYOD_Auto_Switch_Toggle;
        private void onBYOD_Auto_Switch_Toggle(SmartObjectEventArgs e) { var h = BYOD_Auto_Switch_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> BYOD_Auto_Power_Toggle;
        private void onBYOD_Auto_Power_Toggle(SmartObjectEventArgs e) { var h = BYOD_Auto_Power_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Startup_Vol_Set;
        private void onStartup_Vol_Set(SmartObjectEventArgs e) { var h = Startup_Vol_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Theme_Mode_Set;
        private void onTheme_Mode_Set(SmartObjectEventArgs e) { var h = Theme_Mode_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Brand_Color_Set;
        private void onBrand_Color_Set(SmartObjectEventArgs e) { var h = Brand_Color_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Clock_Format_Set;
        private void onClock_Format_Set(SmartObjectEventArgs e) { var h = Clock_Format_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Temp_Unit_Set;
        private void onTemp_Unit_Set(SmartObjectEventArgs e) { var h = Temp_Unit_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void BYOD_Auto_Switch_Fb(SettingsBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.BYOD_Auto_Switch_Fb], this);
        }

        public void BYOD_Auto_Power_Fb(SettingsBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.BYOD_Auto_Power_Fb], this);
        }

        public void Startup_Vol_Fb(SettingsUShortInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Startup_Vol_Fb], this);
        }

        public void Theme_Mode_Fb(SettingsStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Theme_Mode], this);
        }

        public void Brand_Color_Fb(SettingsStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Brand_Color], this);
        }

        public void Clock_Format_Fb(SettingsStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Clock_Format], this);
        }

        public void Temp_Unit_Fb(SettingsStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Temp_Unit], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "Settings", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Reset                   = null;
            BYOD_Auto_Switch_Toggle = null;
            BYOD_Auto_Power_Toggle  = null;
            Startup_Vol_Set         = null;
            Theme_Mode_Set          = null;
            Brand_Color_Set         = null;
            Clock_Format_Set        = null;
            Temp_Unit_Set           = null;
        }

        #endregion
    }
}
