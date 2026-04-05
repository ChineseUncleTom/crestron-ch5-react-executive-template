using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface ISystemInfo
    {
        object UserObject { get; set; }

        void Date_Fb(SystemInfoStringInputSigDelegate callback);
        void Time_Fb(SystemInfoStringInputSigDelegate callback);
        void Label_Fb(SystemInfoStringInputSigDelegate callback);
        void Room_Name_Fb(SystemInfoStringInputSigDelegate callback);
        void Room_Number_Fb(SystemInfoStringInputSigDelegate callback);
        void Help_IT_Phone_Fb(SystemInfoStringInputSigDelegate callback);
        void Help_Support_Email_Fb(SystemInfoStringInputSigDelegate callback);
        void Help_QR_Label_Fb(SystemInfoStringInputSigDelegate callback);
    }

    public delegate void SystemInfoStringInputSigDelegate(StringInputSig stringInputSig, ISystemInfo systemInfo);

    /// <summary>
    /// Read-only room information strings pushed from the processor to the panel.
    /// SmartObjectId: 45
    /// </summary>
    internal class SystemInfo : ISystemInfo, IDisposable
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
            internal static class Strings
            {
                // States (CS → UI) — all read-only
                public const uint Date_Fb                = 1;
                public const uint Time_Fb                = 2;
                public const uint Label_Fb               = 3;
                public const uint Room_Name_Fb           = 4;
                public const uint Room_Number_Fb         = 5;
                public const uint Help_IT_Phone_Fb       = 6;
                public const uint Help_Support_Email_Fb  = 7;
                public const uint Help_QR_Label_Fb       = 8;
            }
        }

        #endregion

        #region Construction and Initialization

        internal SystemInfo(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();
            // No events to configure — this component is processor → panel only.
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

        public void Date_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Date_Fb], this);
        }

        public void Time_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Time_Fb], this);
        }

        public void Label_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Label_Fb], this);
        }

        public void Room_Name_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Room_Name_Fb], this);
        }

        public void Room_Number_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Room_Number_Fb], this);
        }

        public void Help_IT_Phone_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Help_IT_Phone_Fb], this);
        }

        public void Help_Support_Email_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Help_Support_Email_Fb], this);
        }

        public void Help_QR_Label_Fb(SystemInfoStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Help_QR_Label_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "SystemInfo", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
        }

        #endregion
    }
}
