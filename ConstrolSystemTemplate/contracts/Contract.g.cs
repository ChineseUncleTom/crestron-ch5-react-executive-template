using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    /// <summary>
    /// Common Interface for Root Contracts.
    /// </summary>
    public interface IContract
    {
        object UserObject { get; set; }
        void AddDevice(BasicTriListWithSmartObject device);
        void RemoveDevice(BasicTriListWithSmartObject device);
    }

    /// <summary>
    /// Full executive room template contract.
    /// Covers all 46 components defined in ExecutiveContract.cse2j.
    /// </summary>
    public class Contract : IContract, IDisposable
    {
        #region Components

        private ComponentMediator ComponentMediator { get; set; }

        // ── Unique components ─────────────────────────────────────────────────
        public ISystemControl   SystemControl { get { return InternalSystemControl; } }
        public ILighting        Lighting      { get { return InternalLighting; } }
        public IMasterVolume    MasterVolume  { get { return InternalMasterVolume; } }
        public IAudioPrivacy    AudioPrivacy  { get { return InternalAudioPrivacy; } }
        public ICamera          Camera        { get { return InternalCamera; } }
        public ISystemInfo      SystemInfo    { get { return InternalSystemInfo; } }
        public ISettings        Settings      { get { return InternalSettings; } }

        private SystemControl  InternalSystemControl { get; set; }
        private Lighting       InternalLighting      { get; set; }
        private MasterVolume   InternalMasterVolume  { get; set; }
        private AudioPrivacy   InternalAudioPrivacy  { get; set; }
        private Camera         InternalCamera        { get; set; }
        private SystemInfo     InternalSystemInfo    { get; set; }
        private Settings       InternalSettings      { get; set; }

        // ── Video sources (1–5, smartObjectIds 5–9) ──────────────────────────
        public IVideoSource VideoSource1 { get { return InternalVideoSources[0]; } }
        public IVideoSource VideoSource2 { get { return InternalVideoSources[1]; } }
        public IVideoSource VideoSource3 { get { return InternalVideoSources[2]; } }
        public IVideoSource VideoSource4 { get { return InternalVideoSources[3]; } }
        public IVideoSource VideoSource5 { get { return InternalVideoSources[4]; } }
        private VideoSource[] InternalVideoSources { get; set; }

        // ── Video destinations (1–9, smartObjectIds 10–18) ───────────────────
        public IVideoDestination VideoDestination1 { get { return InternalVideoDestinations[0]; } }
        public IVideoDestination VideoDestination2 { get { return InternalVideoDestinations[1]; } }
        public IVideoDestination VideoDestination3 { get { return InternalVideoDestinations[2]; } }
        public IVideoDestination VideoDestination4 { get { return InternalVideoDestinations[3]; } }
        public IVideoDestination VideoDestination5 { get { return InternalVideoDestinations[4]; } }
        public IVideoDestination VideoDestination6 { get { return InternalVideoDestinations[5]; } }
        public IVideoDestination VideoDestination7 { get { return InternalVideoDestinations[6]; } }
        public IVideoDestination VideoDestination8 { get { return InternalVideoDestinations[7]; } }
        public IVideoDestination VideoDestination9 { get { return InternalVideoDestinations[8]; } }
        private VideoDestination[] InternalVideoDestinations { get; set; }

        // ── Audio sources (1–5, smartObjectIds 19–23) ────────────────────────
        public IAudioSource AudioSource1 { get { return InternalAudioSources[0]; } }
        public IAudioSource AudioSource2 { get { return InternalAudioSources[1]; } }
        public IAudioSource AudioSource3 { get { return InternalAudioSources[2]; } }
        public IAudioSource AudioSource4 { get { return InternalAudioSources[3]; } }
        public IAudioSource AudioSource5 { get { return InternalAudioSources[4]; } }
        private AudioSource[] InternalAudioSources { get; set; }

        // ── Camera presets (1–10, smartObjectIds 25–34) ──────────────────────
        public ICameraPreset CameraPreset1  { get { return InternalCameraPresets[0]; } }
        public ICameraPreset CameraPreset2  { get { return InternalCameraPresets[1]; } }
        public ICameraPreset CameraPreset3  { get { return InternalCameraPresets[2]; } }
        public ICameraPreset CameraPreset4  { get { return InternalCameraPresets[3]; } }
        public ICameraPreset CameraPreset5  { get { return InternalCameraPresets[4]; } }
        public ICameraPreset CameraPreset6  { get { return InternalCameraPresets[5]; } }
        public ICameraPreset CameraPreset7  { get { return InternalCameraPresets[6]; } }
        public ICameraPreset CameraPreset8  { get { return InternalCameraPresets[7]; } }
        public ICameraPreset CameraPreset9  { get { return InternalCameraPresets[8]; } }
        public ICameraPreset CameraPreset10 { get { return InternalCameraPresets[9]; } }
        private CameraPreset[] InternalCameraPresets { get; set; }

        // ── Camera selects (1–10, smartObjectIds 35–44) ──────────────────────
        public ICameraSelect CameraSelect1  { get { return InternalCameraSelects[0]; } }
        public ICameraSelect CameraSelect2  { get { return InternalCameraSelects[1]; } }
        public ICameraSelect CameraSelect3  { get { return InternalCameraSelects[2]; } }
        public ICameraSelect CameraSelect4  { get { return InternalCameraSelects[3]; } }
        public ICameraSelect CameraSelect5  { get { return InternalCameraSelects[4]; } }
        public ICameraSelect CameraSelect6  { get { return InternalCameraSelects[5]; } }
        public ICameraSelect CameraSelect7  { get { return InternalCameraSelects[6]; } }
        public ICameraSelect CameraSelect8  { get { return InternalCameraSelects[7]; } }
        public ICameraSelect CameraSelect9  { get { return InternalCameraSelects[8]; } }
        public ICameraSelect CameraSelect10 { get { return InternalCameraSelects[9]; } }
        private CameraSelect[] InternalCameraSelects { get; set; }

        #endregion

        #region Construction and Initialization

        public Contract()
            : this(new BasicTriListWithSmartObject[0])
        {
        }

        public Contract(BasicTriListWithSmartObject device)
            : this(new[] { device })
        {
        }

        public Contract(BasicTriListWithSmartObject[] devices)
        {
            if (devices == null)
                throw new ArgumentNullException("devices");

            ComponentMediator = new ComponentMediator();

            // ── Unique components (smartObjectIds 1–4, 24, 45–46) ─────────────
            InternalSystemControl = new SystemControl(ComponentMediator, 1);
            InternalLighting      = new Lighting(ComponentMediator, 2);
            InternalMasterVolume  = new MasterVolume(ComponentMediator, 3);
            InternalAudioPrivacy  = new AudioPrivacy(ComponentMediator, 4);
            InternalCamera        = new Camera(ComponentMediator, 24);
            InternalSystemInfo    = new SystemInfo(ComponentMediator, 45);
            InternalSettings      = new Settings(ComponentMediator, 46);

            // ── Video sources (smartObjectIds 5–9) ────────────────────────────
            InternalVideoSources = new VideoSource[5];
            for (int i = 0; i < 5; i++)
                InternalVideoSources[i] = new VideoSource(ComponentMediator, (uint)(5 + i));

            // ── Video destinations (smartObjectIds 10–18) ─────────────────────
            InternalVideoDestinations = new VideoDestination[9];
            for (int i = 0; i < 9; i++)
                InternalVideoDestinations[i] = new VideoDestination(ComponentMediator, (uint)(10 + i));

            // ── Audio sources (smartObjectIds 19–23) ──────────────────────────
            InternalAudioSources = new AudioSource[5];
            for (int i = 0; i < 5; i++)
                InternalAudioSources[i] = new AudioSource(ComponentMediator, (uint)(19 + i));

            // ── Camera presets (smartObjectIds 25–34) ─────────────────────────
            InternalCameraPresets = new CameraPreset[10];
            for (int i = 0; i < 10; i++)
                InternalCameraPresets[i] = new CameraPreset(ComponentMediator, (uint)(25 + i));

            // ── Camera selects (smartObjectIds 35–44) ─────────────────────────
            InternalCameraSelects = new CameraSelect[10];
            for (int i = 0; i < 10; i++)
                InternalCameraSelects[i] = new CameraSelect(ComponentMediator, (uint)(35 + i));

            for (int index = 0; index < devices.Length; index++)
                AddDevice(devices[index]);
        }

        #endregion

        #region Standard Contract Members

        public object UserObject { get; set; }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            InternalSystemControl.AddDevice(device);
            InternalLighting.AddDevice(device);
            InternalMasterVolume.AddDevice(device);
            InternalAudioPrivacy.AddDevice(device);
            InternalCamera.AddDevice(device);
            InternalSystemInfo.AddDevice(device);
            InternalSettings.AddDevice(device);

            for (int i = 0; i < InternalVideoSources.Length; i++)
                InternalVideoSources[i].AddDevice(device);
            for (int i = 0; i < InternalVideoDestinations.Length; i++)
                InternalVideoDestinations[i].AddDevice(device);
            for (int i = 0; i < InternalAudioSources.Length; i++)
                InternalAudioSources[i].AddDevice(device);
            for (int i = 0; i < InternalCameraPresets.Length; i++)
                InternalCameraPresets[i].AddDevice(device);
            for (int i = 0; i < InternalCameraSelects.Length; i++)
                InternalCameraSelects[i].AddDevice(device);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            InternalSystemControl.RemoveDevice(device);
            InternalLighting.RemoveDevice(device);
            InternalMasterVolume.RemoveDevice(device);
            InternalAudioPrivacy.RemoveDevice(device);
            InternalCamera.RemoveDevice(device);
            InternalSystemInfo.RemoveDevice(device);
            InternalSettings.RemoveDevice(device);

            for (int i = 0; i < InternalVideoSources.Length; i++)
                InternalVideoSources[i].RemoveDevice(device);
            for (int i = 0; i < InternalVideoDestinations.Length; i++)
                InternalVideoDestinations[i].RemoveDevice(device);
            for (int i = 0; i < InternalAudioSources.Length; i++)
                InternalAudioSources[i].RemoveDevice(device);
            for (int i = 0; i < InternalCameraPresets.Length; i++)
                InternalCameraPresets[i].RemoveDevice(device);
            for (int i = 0; i < InternalCameraSelects.Length; i++)
                InternalCameraSelects[i].RemoveDevice(device);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            InternalSystemControl.Dispose();
            InternalLighting.Dispose();
            InternalMasterVolume.Dispose();
            InternalAudioPrivacy.Dispose();
            InternalCamera.Dispose();
            InternalSystemInfo.Dispose();
            InternalSettings.Dispose();

            for (int i = 0; i < InternalVideoSources.Length; i++)
                InternalVideoSources[i].Dispose();
            for (int i = 0; i < InternalVideoDestinations.Length; i++)
                InternalVideoDestinations[i].Dispose();
            for (int i = 0; i < InternalAudioSources.Length; i++)
                InternalAudioSources[i].Dispose();
            for (int i = 0; i < InternalCameraPresets.Length; i++)
                InternalCameraPresets[i].Dispose();
            for (int i = 0; i < InternalCameraSelects.Length; i++)
                InternalCameraSelects[i].Dispose();

            ComponentMediator.Dispose();
        }

        #endregion
    }
}
