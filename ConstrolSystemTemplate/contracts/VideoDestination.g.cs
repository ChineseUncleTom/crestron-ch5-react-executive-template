using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface IVideoDestination
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Select;
        event EventHandler<UIEventArgs> Power_Toggle;
        event EventHandler<UIEventArgs> Video_Toggle;

        void Active_Fb(VideoDestinationBoolInputSigDelegate callback);
        void Power_Fb(VideoDestinationBoolInputSigDelegate callback);
        void Video_Fb(VideoDestinationBoolInputSigDelegate callback);
        void Name_Fb(VideoDestinationStringInputSigDelegate callback);
        void Routed_Source_Fb(VideoDestinationStringInputSigDelegate callback);
    }

    public delegate void VideoDestinationBoolInputSigDelegate(BoolInputSig boolInputSig, IVideoDestination videoDestination);
    public delegate void VideoDestinationStringInputSigDelegate(StringInputSig stringInputSig, IVideoDestination videoDestination);

    /// <summary>
    /// Video destination – select, power and video toggle events; active/power/video
    /// feedback and display/routed-source name strings.
    /// Reused for VideoDestination1 (smartObjectId 10) through VideoDestination9 (smartObjectId 18).
    /// </summary>
    internal class VideoDestination : IVideoDestination, IDisposable
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
                public const uint Select       = 1;
                public const uint Power_Toggle = 2;
                public const uint Video_Toggle = 3;

                // States (CS → UI)
                public const uint Active_Fb    = 1;
                public const uint Power_Fb     = 2;
                public const uint Video_Fb     = 3;
            }
            internal static class Strings
            {
                // States (CS → UI)
                public const uint Name_Fb            = 1;
                public const uint Routed_Source_Fb   = 2;
            }
        }

        #endregion

        #region Construction and Initialization

        internal VideoDestination(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Select,       onSelect);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Power_Toggle, onPower_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Video_Toggle, onVideo_Toggle);
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

        public event EventHandler<UIEventArgs> Power_Toggle;
        private void onPower_Toggle(SmartObjectEventArgs e) { var h = Power_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Video_Toggle;
        private void onVideo_Toggle(SmartObjectEventArgs e) { var h = Video_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Active_Fb(VideoDestinationBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Active_Fb], this);
        }

        public void Power_Fb(VideoDestinationBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Power_Fb], this);
        }

        public void Video_Fb(VideoDestinationBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Video_Fb], this);
        }

        public void Name_Fb(VideoDestinationStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Name_Fb], this);
        }

        public void Routed_Source_Fb(VideoDestinationStringInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Routed_Source_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "VideoDestination", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Select       = null;
            Power_Toggle = null;
            Video_Toggle = null;
        }

        #endregion
    }
}
