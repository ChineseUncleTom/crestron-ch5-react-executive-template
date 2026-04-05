using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    public interface ICamera
    {
        object UserObject { get; set; }

        /// <summary>Hold high while panning left.</summary>
        event EventHandler<UIEventArgs> Pan_Left;
        /// <summary>Hold high while panning right.</summary>
        event EventHandler<UIEventArgs> Pan_Right;
        /// <summary>Hold high while tilting up.</summary>
        event EventHandler<UIEventArgs> Tilt_Up;
        /// <summary>Hold high while tilting down.</summary>
        event EventHandler<UIEventArgs> Tilt_Down;
        /// <summary>Hold high while zooming in.</summary>
        event EventHandler<UIEventArgs> Zoom_In;
        /// <summary>Hold high while zooming out.</summary>
        event EventHandler<UIEventArgs> Zoom_Out;
        event EventHandler<UIEventArgs> Power_Toggle;
        event EventHandler<UIEventArgs> Tracking_Toggle;
        /// <summary>Fired when the UI sends a zoom speed value (0–65535).</summary>
        event EventHandler<UIEventArgs> Zoom_Speed_Set;
        /// <summary>Fired when the UI sends a moment (movement) speed value (0–65535).</summary>
        event EventHandler<UIEventArgs> Moment_Speed_Set;

        void Power_Fb(CameraBoolInputSigDelegate callback);
        void Tracking_Fb(CameraBoolInputSigDelegate callback);
        void Zoom_Speed_Fb(CameraUShortInputSigDelegate callback);
        void Moment_Speed_Fb(CameraUShortInputSigDelegate callback);
    }

    public delegate void CameraBoolInputSigDelegate(BoolInputSig boolInputSig, ICamera camera);
    public delegate void CameraUShortInputSigDelegate(UShortInputSig uShortInputSig, ICamera camera);

    /// <summary>
    /// Active camera PTZ controls, zoom/moment speed, power and auto-tracking.
    /// SmartObjectId: 24
    /// </summary>
    internal class Camera : ICamera, IDisposable
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
                public const uint Pan_Left        = 1;
                public const uint Pan_Right       = 2;
                public const uint Tilt_Up         = 3;
                public const uint Tilt_Down       = 4;
                public const uint Zoom_In         = 5;
                public const uint Zoom_Out        = 6;
                public const uint Power_Toggle    = 7;
                public const uint Tracking_Toggle = 8;

                // States (CS → UI)
                public const uint Power_Fb        = 1;
                public const uint Tracking_Fb     = 2;
            }
            internal static class Numerics
            {
                // Events (UI → CS)
                public const uint Zoom_Speed_Set   = 1;
                public const uint Moment_Speed_Set = 2;

                // States (CS → UI)
                public const uint Zoom_Speed_Fb    = 1;
                public const uint Moment_Speed_Fb  = 2;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Camera(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId;
            _devices = new List<BasicTriListWithSmartObject>();

            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Pan_Left,        onPan_Left);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Pan_Right,       onPan_Right);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Tilt_Up,         onTilt_Up);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Tilt_Down,       onTilt_Down);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Zoom_In,         onZoom_In);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Zoom_Out,        onZoom_Out);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Power_Toggle,    onPower_Toggle);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Tracking_Toggle, onTracking_Toggle);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Zoom_Speed_Set,   onZoom_Speed_Set);
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Moment_Speed_Set, onMoment_Speed_Set);
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

        public event EventHandler<UIEventArgs> Pan_Left;
        private void onPan_Left(SmartObjectEventArgs e) { var h = Pan_Left; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Pan_Right;
        private void onPan_Right(SmartObjectEventArgs e) { var h = Pan_Right; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Tilt_Up;
        private void onTilt_Up(SmartObjectEventArgs e) { var h = Tilt_Up; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Tilt_Down;
        private void onTilt_Down(SmartObjectEventArgs e) { var h = Tilt_Down; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Zoom_In;
        private void onZoom_In(SmartObjectEventArgs e) { var h = Zoom_In; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Zoom_Out;
        private void onZoom_Out(SmartObjectEventArgs e) { var h = Zoom_Out; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Power_Toggle;
        private void onPower_Toggle(SmartObjectEventArgs e) { var h = Power_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Tracking_Toggle;
        private void onTracking_Toggle(SmartObjectEventArgs e) { var h = Tracking_Toggle; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Zoom_Speed_Set;
        private void onZoom_Speed_Set(SmartObjectEventArgs e) { var h = Zoom_Speed_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public event EventHandler<UIEventArgs> Moment_Speed_Set;
        private void onMoment_Speed_Set(SmartObjectEventArgs e) { var h = Moment_Speed_Set; if (h != null) h(this, UIEventArgs.CreateEventArgs(e)); }

        public void Power_Fb(CameraBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Power_Fb], this);
        }

        public void Tracking_Fb(CameraBoolInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Tracking_Fb], this);
        }

        public void Zoom_Speed_Fb(CameraUShortInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Zoom_Speed_Fb], this);
        }

        public void Moment_Speed_Fb(CameraUShortInputSigDelegate callback)
        {
            for (int i = 0; i < Devices.Count; i++)
                callback(Devices[i].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Moment_Speed_Fb], this);
        }

        #endregion

        #region Overrides

        public override int GetHashCode() { return (int)ControlJoinId; }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}",
                "Camera", GetType().Name, GetHashCode(),
                UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Pan_Left        = null;
            Pan_Right       = null;
            Tilt_Up         = null;
            Tilt_Down       = null;
            Zoom_In         = null;
            Zoom_Out        = null;
            Power_Toggle    = null;
            Tracking_Toggle = null;
            Zoom_Speed_Set  = null;
            Moment_Speed_Set = null;
        }

        #endregion
    }
}
