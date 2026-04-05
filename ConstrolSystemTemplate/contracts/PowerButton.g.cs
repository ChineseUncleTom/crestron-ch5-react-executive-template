using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface IPowerButton
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Power_On;
        event EventHandler<UIEventArgs> Power_Off;
        event EventHandler<UIEventArgs> Mode;

        void Power_On_Fb(PowerButtonBoolInputSigDelegate callback);
        void Power_Off_Fb(PowerButtonBoolInputSigDelegate callback);
        void Mode_Fb(PowerButtonUShortInputSigDelegate callback);
        void Name_Fb(PowerButtonStringInputSigDelegate callback);

    }

    public delegate void PowerButtonBoolInputSigDelegate(BoolInputSig boolInputSig, IPowerButton powerButton);
    public delegate void PowerButtonUShortInputSigDelegate(UShortInputSig uShortInputSig, IPowerButton powerButton);
    public delegate void PowerButtonStringInputSigDelegate(StringInputSig stringInputSig, IPowerButton powerButton);

    /// <summary>
    /// Power ON/OFF Button
    /// </summary>
    internal class PowerButton : IPowerButton, IDisposable
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
                public const uint Power_On = 1;
                public const uint Power_Off = 2;

                public const uint Power_On_Fb = 1;
                public const uint Power_Off_Fb = 2;
            }
            internal static class Numerics
            {
                public const uint Mode = 1;

                public const uint Mode_Fb = 1;
            }
            internal static class Strings
            {

                public const uint Name_Fb = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal PowerButton(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Power_On, onPower_On);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Power_Off, onPower_Off);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Mode, onMode);

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

        public event EventHandler<UIEventArgs> Power_On;
        private void onPower_On(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Power_On;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Power_Off;
        private void onPower_Off(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Power_Off;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Power_On_Fb(PowerButtonBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Power_On_Fb], this);
            }
        }

        public void Power_Off_Fb(PowerButtonBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Power_Off_Fb], this);
            }
        }

        public event EventHandler<UIEventArgs> Mode;
        private void onMode(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Mode;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Mode_Fb(PowerButtonUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Mode_Fb], this);
            }
        }


        public void Name_Fb(PowerButtonStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Name_Fb], this);
            }
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return (int)ControlJoinId;
        }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "PowerButton", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            Power_On = null;
            Power_Off = null;
            Mode = null;
        }

        #endregion

    }
}
