using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface ICameraPreset
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Recall;

        void Name_Fb(CameraPresetStringInputSigDelegate callback);
    }

    public delegate void CameraPresetStringInputSigDelegate(StringInputSig stringInputSig, ICameraPreset cameraPreset);

    /// <summary>
    /// Camera preset – recall button and preset name string.
    /// Reused for CameraPreset1 (smartObjectId 25) through CameraPreset10 (smartObjectId 34).
    /// </summary>
    internal class CameraPreset : ICameraPreset, IDisposable
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
                public const uint Recall = 1;
            }
            internal static class Strings
            {
                // States (CS → UI)
                public const uint Name_Fb = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal CameraPreset(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Recall, onRecall);
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

        public event EventHandler<UIEventArgs> Recall;
        private void onRecall(SmartObjectEventArgs e) { var h = Recall; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Name_Fb(CameraPresetStringInputSigDelegate callback)
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
                "CameraPreset", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Recall = null;
        }

        #endregion
    }
}
