/**
 * ContractSignals
 *
 * Typed signal-path constants for every entry in ExecutiveContract.cse2j.
 *
 * Usage with CrComLib (replaces raw join numbers):
 *
 *   // Subscribe to a feedback state (CS → UI)
 *   CrComLib.subscribeState('b', ContractSignals.SystemControl.System_On_Fb, (v) => setRoomOn(v));
 *
 *   // Publish an event (UI → CS)
 *   CrComLib.publishEvent('b', ContractSignals.SystemControl.System_Startup, true);
 *   setTimeout(() => CrComLib.publishEvent('b', ContractSignals.SystemControl.System_Startup, false), 200);
 *
 *   // Subscribe to a serial (string) state
 *   CrComLib.subscribeState('s', ContractSignals.SystemInfo.Room_Name_Fb, (v) => setRoomName(v));
 *
 *   // Subscribe to a numeric (analog) state
 *   CrComLib.subscribeState('n', ContractSignals.MasterVolume.Volume_Fb, (v) => setVolume(v));
 *
 * Signal naming convention:
 *   - Signals ending in _Fb  → feedback from the processor (CS → UI, subscribe only)
 *   - Signals without _Fb   → events from the UI to the processor (UI → CS, publish)
 *   - Bidirectional serials (Settings) share the same path for send and receive
 *
 * Component index table (mirrors cse2j smartObjectId values):
 *   1   SystemControl         | 10–18  VideoDestination 1–9
 *   2   Lighting              | 19–23  AudioSource 1–5
 *   3   MasterVolume          | 24     Camera
 *   4   AudioPrivacy          | 25–34  CameraPreset 1–10
 *   5–9 VideoSource 1–5       | 35–44  CameraSelect 1–10
 *                             | 45     SystemInfo
 *                             | 46     Settings
 */

// ── SystemControl (smartObjectId: 1) ──────────────────────────────────────────
export const SystemControl = {
  // Events (UI → CS)
  System_Startup:    'SystemControl.System_Startup',
  System_Off:        'SystemControl.System_Off',
  Present_To_Room:   'SystemControl.Present_To_Room',
  Ingest_Mode_Toggle: 'SystemControl.Ingest_Mode_Toggle',
  BYOD_Mode_Toggle:  'SystemControl.BYOD_Mode_Toggle',
  BYOD_Select:       'SystemControl.BYOD_Select',
  // States (CS → UI)
  System_On_Fb:      'SystemControl.System_On_Fb',
  Ingest_Mode_Fb:    'SystemControl.Ingest_Mode_Fb',
  BYOD_Mode_Fb:      'SystemControl.BYOD_Mode_Fb',
} as const;

// ── Lighting (smartObjectId: 2) ───────────────────────────────────────────────
export const Lighting = {
  // Events (UI → CS)
  Lights_On:      'Lighting.Lights_On',
  Lights_Off:     'Lighting.Lights_Off',
  Lights_Toggle:  'Lighting.Lights_Toggle',
  Brightness_Set: 'Lighting.Brightness_Set',
  // States (CS → UI)
  Lights_On_Fb:   'Lighting.Lights_On_Fb',
  Brightness_Fb:  'Lighting.Brightness_Fb',
} as const;

// ── MasterVolume (smartObjectId: 3) ───────────────────────────────────────────
export const MasterVolume = {
  // Events (UI → CS)
  Mute_Toggle: 'MasterVolume.Mute_Toggle',
  Default:     'MasterVolume.Default',
  Volume_Set:  'MasterVolume.Volume_Set',
  // States (CS → UI)
  Mute_Fb:     'MasterVolume.Mute_Fb',
  Volume_Fb:   'MasterVolume.Volume_Fb',
} as const;

// ── AudioPrivacy (smartObjectId: 4) ───────────────────────────────────────────
export const AudioPrivacy = {
  // Events (UI → CS)
  Privacy_Mute_Toggle:       'AudioPrivacy.Privacy_Mute_Toggle',
  Wireless_Mic_Mute_Toggle:  'AudioPrivacy.Wireless_Mic_Mute_Toggle',
  Ceiling_Mic_Mute_Toggle:   'AudioPrivacy.Ceiling_Mic_Mute_Toggle',
  // States (CS → UI)
  Privacy_Mute_Fb:           'AudioPrivacy.Privacy_Mute_Fb',
  Wireless_Mic_Mute_Fb:      'AudioPrivacy.Wireless_Mic_Mute_Fb',
  Ceiling_Mic_Mute_Fb:       'AudioPrivacy.Ceiling_Mic_Mute_Fb',
} as const;

// ── VideoSource (smartObjectIds: 5–9) ─────────────────────────────────────────
// Each entry is an object with the signal paths for that source instance.
export const VideoSource = [
  {
    Select:       'VideoSource1.Select',
    Active_Fb:    'VideoSource1.Active_Fb',
    Available_Fb: 'VideoSource1.Available_Fb',
    Name_Fb:      'VideoSource1.Name_Fb',
  },
  {
    Select:       'VideoSource2.Select',
    Active_Fb:    'VideoSource2.Active_Fb',
    Available_Fb: 'VideoSource2.Available_Fb',
    Name_Fb:      'VideoSource2.Name_Fb',
  },
  {
    Select:       'VideoSource3.Select',
    Active_Fb:    'VideoSource3.Active_Fb',
    Available_Fb: 'VideoSource3.Available_Fb',
    Name_Fb:      'VideoSource3.Name_Fb',
  },
  {
    Select:       'VideoSource4.Select',
    Active_Fb:    'VideoSource4.Active_Fb',
    Available_Fb: 'VideoSource4.Available_Fb',
    Name_Fb:      'VideoSource4.Name_Fb',
  },
  {
    Select:       'VideoSource5.Select',
    Active_Fb:    'VideoSource5.Active_Fb',
    Available_Fb: 'VideoSource5.Available_Fb',
    Name_Fb:      'VideoSource5.Name_Fb',
  },
] as const;

// ── VideoDestination (smartObjectIds: 10–18) ──────────────────────────────────
export const VideoDestination = [
  {
    Select:          'VideoDestination1.Select',
    Power_Toggle:    'VideoDestination1.Power_Toggle',
    Video_Toggle:    'VideoDestination1.Video_Toggle',
    Active_Fb:       'VideoDestination1.Active_Fb',
    Power_Fb:        'VideoDestination1.Power_Fb',
    Video_Fb:        'VideoDestination1.Video_Fb',
    Name_Fb:         'VideoDestination1.Name_Fb',
    Routed_Source_Fb:'VideoDestination1.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination2.Select',
    Power_Toggle:    'VideoDestination2.Power_Toggle',
    Video_Toggle:    'VideoDestination2.Video_Toggle',
    Active_Fb:       'VideoDestination2.Active_Fb',
    Power_Fb:        'VideoDestination2.Power_Fb',
    Video_Fb:        'VideoDestination2.Video_Fb',
    Name_Fb:         'VideoDestination2.Name_Fb',
    Routed_Source_Fb:'VideoDestination2.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination3.Select',
    Power_Toggle:    'VideoDestination3.Power_Toggle',
    Video_Toggle:    'VideoDestination3.Video_Toggle',
    Active_Fb:       'VideoDestination3.Active_Fb',
    Power_Fb:        'VideoDestination3.Power_Fb',
    Video_Fb:        'VideoDestination3.Video_Fb',
    Name_Fb:         'VideoDestination3.Name_Fb',
    Routed_Source_Fb:'VideoDestination3.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination4.Select',
    Power_Toggle:    'VideoDestination4.Power_Toggle',
    Video_Toggle:    'VideoDestination4.Video_Toggle',
    Active_Fb:       'VideoDestination4.Active_Fb',
    Power_Fb:        'VideoDestination4.Power_Fb',
    Video_Fb:        'VideoDestination4.Video_Fb',
    Name_Fb:         'VideoDestination4.Name_Fb',
    Routed_Source_Fb:'VideoDestination4.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination5.Select',
    Power_Toggle:    'VideoDestination5.Power_Toggle',
    Video_Toggle:    'VideoDestination5.Video_Toggle',
    Active_Fb:       'VideoDestination5.Active_Fb',
    Power_Fb:        'VideoDestination5.Power_Fb',
    Video_Fb:        'VideoDestination5.Video_Fb',
    Name_Fb:         'VideoDestination5.Name_Fb',
    Routed_Source_Fb:'VideoDestination5.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination6.Select',
    Power_Toggle:    'VideoDestination6.Power_Toggle',
    Video_Toggle:    'VideoDestination6.Video_Toggle',
    Active_Fb:       'VideoDestination6.Active_Fb',
    Power_Fb:        'VideoDestination6.Power_Fb',
    Video_Fb:        'VideoDestination6.Video_Fb',
    Name_Fb:         'VideoDestination6.Name_Fb',
    Routed_Source_Fb:'VideoDestination6.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination7.Select',
    Power_Toggle:    'VideoDestination7.Power_Toggle',
    Video_Toggle:    'VideoDestination7.Video_Toggle',
    Active_Fb:       'VideoDestination7.Active_Fb',
    Power_Fb:        'VideoDestination7.Power_Fb',
    Video_Fb:        'VideoDestination7.Video_Fb',
    Name_Fb:         'VideoDestination7.Name_Fb',
    Routed_Source_Fb:'VideoDestination7.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination8.Select',
    Power_Toggle:    'VideoDestination8.Power_Toggle',
    Video_Toggle:    'VideoDestination8.Video_Toggle',
    Active_Fb:       'VideoDestination8.Active_Fb',
    Power_Fb:        'VideoDestination8.Power_Fb',
    Video_Fb:        'VideoDestination8.Video_Fb',
    Name_Fb:         'VideoDestination8.Name_Fb',
    Routed_Source_Fb:'VideoDestination8.Routed_Source_Fb',
  },
  {
    Select:          'VideoDestination9.Select',
    Power_Toggle:    'VideoDestination9.Power_Toggle',
    Video_Toggle:    'VideoDestination9.Video_Toggle',
    Active_Fb:       'VideoDestination9.Active_Fb',
    Power_Fb:        'VideoDestination9.Power_Fb',
    Video_Fb:        'VideoDestination9.Video_Fb',
    Name_Fb:         'VideoDestination9.Name_Fb',
    Routed_Source_Fb:'VideoDestination9.Routed_Source_Fb',
  },
] as const;

// ── AudioSource (smartObjectIds: 19–23) ───────────────────────────────────────
export const AudioSource = [
  {
    Vol_Up:      'AudioSource1.Vol_Up',
    Vol_Down:    'AudioSource1.Vol_Down',
    Default:     'AudioSource1.Default',
    Mute_Toggle: 'AudioSource1.Mute_Toggle',
    Volume_Set:  'AudioSource1.Volume_Set',
    Mute_Fb:     'AudioSource1.Mute_Fb',
    Volume_Fb:   'AudioSource1.Volume_Fb',
    Name_Fb:     'AudioSource1.Name_Fb',
  },
  {
    Vol_Up:      'AudioSource2.Vol_Up',
    Vol_Down:    'AudioSource2.Vol_Down',
    Default:     'AudioSource2.Default',
    Mute_Toggle: 'AudioSource2.Mute_Toggle',
    Volume_Set:  'AudioSource2.Volume_Set',
    Mute_Fb:     'AudioSource2.Mute_Fb',
    Volume_Fb:   'AudioSource2.Volume_Fb',
    Name_Fb:     'AudioSource2.Name_Fb',
  },
  {
    Vol_Up:      'AudioSource3.Vol_Up',
    Vol_Down:    'AudioSource3.Vol_Down',
    Default:     'AudioSource3.Default',
    Mute_Toggle: 'AudioSource3.Mute_Toggle',
    Volume_Set:  'AudioSource3.Volume_Set',
    Mute_Fb:     'AudioSource3.Mute_Fb',
    Volume_Fb:   'AudioSource3.Volume_Fb',
    Name_Fb:     'AudioSource3.Name_Fb',
  },
  {
    Vol_Up:      'AudioSource4.Vol_Up',
    Vol_Down:    'AudioSource4.Vol_Down',
    Default:     'AudioSource4.Default',
    Mute_Toggle: 'AudioSource4.Mute_Toggle',
    Volume_Set:  'AudioSource4.Volume_Set',
    Mute_Fb:     'AudioSource4.Mute_Fb',
    Volume_Fb:   'AudioSource4.Volume_Fb',
    Name_Fb:     'AudioSource4.Name_Fb',
  },
  {
    Vol_Up:      'AudioSource5.Vol_Up',
    Vol_Down:    'AudioSource5.Vol_Down',
    Default:     'AudioSource5.Default',
    Mute_Toggle: 'AudioSource5.Mute_Toggle',
    Volume_Set:  'AudioSource5.Volume_Set',
    Mute_Fb:     'AudioSource5.Mute_Fb',
    Volume_Fb:   'AudioSource5.Volume_Fb',
    Name_Fb:     'AudioSource5.Name_Fb',
  },
] as const;

// ── Camera (smartObjectId: 24) ────────────────────────────────────────────────
export const CameraControl = {
  // Events (UI → CS) – PTZ buttons are holds (true on press, false on release)
  Pan_Left:        'Camera.Pan_Left',
  Pan_Right:       'Camera.Pan_Right',
  Tilt_Up:         'Camera.Tilt_Up',
  Tilt_Down:       'Camera.Tilt_Down',
  Zoom_In:         'Camera.Zoom_In',
  Zoom_Out:        'Camera.Zoom_Out',
  Power_Toggle:      'Camera.Power_Toggle',
  Tracking_Toggle:   'Camera.Tracking_Toggle',
  Zoom_Speed_Set:    'Camera.Zoom_Speed_Set',
  Movement_Speed_Set:'Camera.Moment_Speed_Set',
  // States (CS → UI)
  Power_Fb:          'Camera.Power_Fb',
  Tracking_Fb:       'Camera.Tracking_Fb',
  Zoom_Speed_Fb:     'Camera.Zoom_Speed_Fb',
  Movement_Speed_Fb: 'Camera.Moment_Speed_Fb',
} as const;

// ── CameraPreset (smartObjectIds: 25–34) ──────────────────────────────────────
export const CameraPreset = [
  { Recall: 'CameraPreset1.Recall',  Name_Fb: 'CameraPreset1.Name_Fb'  },
  { Recall: 'CameraPreset2.Recall',  Name_Fb: 'CameraPreset2.Name_Fb'  },
  { Recall: 'CameraPreset3.Recall',  Name_Fb: 'CameraPreset3.Name_Fb'  },
  { Recall: 'CameraPreset4.Recall',  Name_Fb: 'CameraPreset4.Name_Fb'  },
  { Recall: 'CameraPreset5.Recall',  Name_Fb: 'CameraPreset5.Name_Fb'  },
  { Recall: 'CameraPreset6.Recall',  Name_Fb: 'CameraPreset6.Name_Fb'  },
  { Recall: 'CameraPreset7.Recall',  Name_Fb: 'CameraPreset7.Name_Fb'  },
  { Recall: 'CameraPreset8.Recall',  Name_Fb: 'CameraPreset8.Name_Fb'  },
  { Recall: 'CameraPreset9.Recall',  Name_Fb: 'CameraPreset9.Name_Fb'  },
  { Recall: 'CameraPreset10.Recall', Name_Fb: 'CameraPreset10.Name_Fb' },
] as const;

// ── CameraSelect (smartObjectIds: 35–44) ──────────────────────────────────────
export const CameraSelect = [
  { Select: 'CameraSelect1.Select',  Active_Fb: 'CameraSelect1.Active_Fb',  Name_Fb: 'CameraSelect1.Name_Fb'  },
  { Select: 'CameraSelect2.Select',  Active_Fb: 'CameraSelect2.Active_Fb',  Name_Fb: 'CameraSelect2.Name_Fb'  },
  { Select: 'CameraSelect3.Select',  Active_Fb: 'CameraSelect3.Active_Fb',  Name_Fb: 'CameraSelect3.Name_Fb'  },
  { Select: 'CameraSelect4.Select',  Active_Fb: 'CameraSelect4.Active_Fb',  Name_Fb: 'CameraSelect4.Name_Fb'  },
  { Select: 'CameraSelect5.Select',  Active_Fb: 'CameraSelect5.Active_Fb',  Name_Fb: 'CameraSelect5.Name_Fb'  },
  { Select: 'CameraSelect6.Select',  Active_Fb: 'CameraSelect6.Active_Fb',  Name_Fb: 'CameraSelect6.Name_Fb'  },
  { Select: 'CameraSelect7.Select',  Active_Fb: 'CameraSelect7.Active_Fb',  Name_Fb: 'CameraSelect7.Name_Fb'  },
  { Select: 'CameraSelect8.Select',  Active_Fb: 'CameraSelect8.Active_Fb',  Name_Fb: 'CameraSelect8.Name_Fb'  },
  { Select: 'CameraSelect9.Select',  Active_Fb: 'CameraSelect9.Active_Fb',  Name_Fb: 'CameraSelect9.Name_Fb'  },
  { Select: 'CameraSelect10.Select', Active_Fb: 'CameraSelect10.Active_Fb', Name_Fb: 'CameraSelect10.Name_Fb' },
] as const;

// ── SystemInfo (smartObjectId: 45) ────────────────────────────────────────────
export const SystemInfo = {
  // States (CS → UI) — all read-only
  Date_Fb:               'SystemInfo.Date_Fb',
  Time_Fb:               'SystemInfo.Time_Fb',
  Label_Fb:              'SystemInfo.Label_Fb',
  Room_Name_Fb:          'SystemInfo.Room_Name_Fb',
  Room_Number_Fb:        'SystemInfo.Room_Number_Fb',
  Help_IT_Phone_Fb:      'SystemInfo.Help_IT_Phone_Fb',
  Help_Support_Email_Fb: 'SystemInfo.Help_Support_Email_Fb',
  Help_QR_Label_Fb:      'SystemInfo.Help_QR_Label_Fb',
} as const;

// ── Settings (smartObjectId: 46) ──────────────────────────────────────────────
export const SettingsSignals = {
  // Events (UI → CS)
  Reset:                  'Settings.Reset',
  BYOD_Auto_Switch_Toggle:'Settings.BYOD_Auto_Switch_Toggle',
  BYOD_Auto_Power_Toggle: 'Settings.BYOD_Auto_Power_Toggle',
  Startup_Vol_Set:        'Settings.Startup_Vol_Set',
  // Bidirectional serials – subscribe with the _Fb path; publish with the dedicated _Set path below
  Theme_Mode_Fb_Path:  'Settings.Theme_Mode_Fb',   // subscribe only
  Brand_Color_Fb_Path: 'Settings.Brand_Color_Fb',  // subscribe only
  Clock_Format_Fb_Path:'Settings.Clock_Format_Fb', // subscribe only
  Temp_Unit_Fb_Path:   'Settings.Temp_Unit_Fb',    // subscribe only
  // Dedicated send paths (as defined in cse2j events.string)
  Theme_Mode_Set:         'Settings.Theme_Mode_Set',
  Brand_Color_Set:        'Settings.Brand_Color_Set',
  Clock_Format_Set:       'Settings.Clock_Format_Set',
  Temp_Unit_Set:          'Settings.Temp_Unit_Set',
  // States (CS → UI)
  BYOD_Auto_Switch_Fb:    'Settings.BYOD_Auto_Switch_Fb',
  BYOD_Auto_Power_Fb:     'Settings.BYOD_Auto_Power_Fb',
  Startup_Vol_Fb:         'Settings.Startup_Vol_Fb',
  Theme_Mode_Fb:          'Settings.Theme_Mode_Fb',
  Brand_Color_Fb:         'Settings.Brand_Color_Fb',
  Clock_Format_Fb:        'Settings.Clock_Format_Fb',
  Temp_Unit_Fb:           'Settings.Temp_Unit_Fb',
} as const;

/**
 * Flat namespace alias – import everything under one name if preferred:
 *
 *   import { ContractSignals as CS } from './ContractSignals';
 *   CrComLib.subscribeState('b', CS.SystemControl.System_On_Fb, handler);
 *   CrComLib.subscribeState('s', CS.SystemInfo.Room_Name_Fb, handler);
 */
export const ContractSignals = {
  SystemControl,
  Lighting,
  MasterVolume,
  AudioPrivacy,
  VideoSource,
  VideoDestination,
  AudioSource,
  Camera: CameraControl,
  CameraPreset,
  CameraSelect,
  SystemInfo,
  Settings: SettingsSignals,
} as const;
