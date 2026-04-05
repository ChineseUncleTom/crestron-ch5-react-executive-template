using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface ICameraSelect
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Select;

        void Active_Fb(CameraSelectBoolInputSigDelegate callback);
        void Name_Fb(CameraSelectStringInputSigDelegate callback);
    }

    public delegate void CameraSelectBoolInputSigDelegate(BoolInputSig boolInputSig, ICameraSelect cameraSelect);
    public delegate void CameraSelectStringInputSigDelegate(StringInputSig stringInputSig, ICameraSelect cameraSelect);

    /// <summary>
    /// Camera select – select button, active feedback, and camera name string.
    /// Reused for CameraSelect1 (smartObjectId 35) through CameraSelect10 (smartObjectId 44).
    /// </summary>
    internal class CameraSelect : ICameraSelect, IDisposable
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
                public const uint Select    = 1;

                // States (CS → UI)
                public const uint Active_Fb = 1;
            }
            internal static class Strings
            {
                // States (CS → UI)
                public const uint Name_Fb = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal CameraSelect(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Select, onSelect);
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

        public event EventHandler<UIEventArgs> Select;
        private void onSelect(SmartObjectEventArgs e) { var h = Select; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Active_Fb(CameraSelectBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Active_Fb], this);
        }

        public void Name_Fb(CameraSelectStringInputSigDelegate callback)
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
                "CameraSelect", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Select = null;
        }

        #endregion
    }
}
