import { useState, useEffect, useCallback } from 'react';
// Import from the CJS bundle directly because @crestron/ch5-crcomlib does not
// declare a package.json "main" field; webpack resolves explicit sub-paths.
// The bundle exposes everything via the named `CrComLib` export.
import { CrComLib } from "@crestron/ch5-crcomlib/build_bundles/cjs/cr-com-lib";

// Expose CrComLib as a global so that the WebXPanel worker thread can
// resolve it by name when routing feedback signals to subscribers.
(window as any).CrComLib = CrComLib;

(window as any)["bridgeReceiveIntegerFromNative"] =
  (CrComLib as any).bridgeReceiveIntegerFromNative;
(window as any)["bridgeReceiveBooleanFromNative"] =
  (CrComLib as any).bridgeReceiveBooleanFromNative;
(window as any)["bridgeReceiveStringFromNative"] =
  (CrComLib as any).bridgeReceiveStringFromNative;
(window as any)["bridgeReceiveObjectFromNative"] =
  (CrComLib as any).bridgeReceiveObjectFromNative;

// ── Join number constants (mirror of control-system/JoinMap.cs) ──────────────
export const Joins = {
  // Digital – lighting
  LIGHT_ON: 1,
  LIGHT_OFF: 2,
  LIGHT_IS_ON: 3,
  LIGHT_TOGGLE: 4,

  // Digital – executive scenario macros (joins 5, 8–10)
  SYSTEM_OFF_FB: 5,         // feedback from processor – room is currently powered on (high = on)
  SYSTEM_STARTUP_BTN: 8,
  SYSTEM_OFF_BTN: 9,
  PRESENT_TO_ROOM_BTN: 10,

  // Digital – video source selection (output: joins 11–15)
  VIDEO_SRC_SELECT_1: 11,
  VIDEO_SRC_SELECT_2: 12,
  VIDEO_SRC_SELECT_3: 13,
  VIDEO_SRC_SELECT_4: 14,
  VIDEO_SRC_SELECT_5: 15,

  // Digital – video source active feedback (from processor: joins 16–20)
  VIDEO_SRC_ACTIVE_1: 16,
  VIDEO_SRC_ACTIVE_2: 17,
  VIDEO_SRC_ACTIVE_3: 18,
  VIDEO_SRC_ACTIVE_4: 19,
  VIDEO_SRC_ACTIVE_5: 20,

  // Digital – destination select (output: joins 46–49)
  VIDEO_DEST_SELECT_1: 46,
  VIDEO_DEST_SELECT_2: 47,
  VIDEO_DEST_SELECT_3: 48,
  VIDEO_DEST_SELECT_4: 49,

  // Digital – destination active feedback (from processor: joins 50–53)
  VIDEO_DEST_ACTIVE_1: 50,
  VIDEO_DEST_ACTIVE_2: 51,
  VIDEO_DEST_ACTIVE_3: 52,
  VIDEO_DEST_ACTIVE_4: 53,

  // Digital – destination power toggle (output: joins 21–24)
  VIDEO_DEST_POWER_BTN_1: 21,
  VIDEO_DEST_POWER_BTN_2: 22,
  VIDEO_DEST_POWER_BTN_3: 23,
  VIDEO_DEST_POWER_BTN_4: 24,

  // Digital – destination power feedback (from processor: joins 25–28)
  VIDEO_DEST_POWER_FB_1: 25,
  VIDEO_DEST_POWER_FB_2: 26,
  VIDEO_DEST_POWER_FB_3: 27,
  VIDEO_DEST_POWER_FB_4: 28,

  // Digital – destination video toggle (output: joins 31–34)
  VIDEO_DEST_VIDEO_BTN_1: 31,
  VIDEO_DEST_VIDEO_BTN_2: 32,
  VIDEO_DEST_VIDEO_BTN_3: 33,
  VIDEO_DEST_VIDEO_BTN_4: 34,

  // Digital – destination video feedback (from processor: joins 35–38)
  VIDEO_DEST_VIDEO_FB_1: 35,
  VIDEO_DEST_VIDEO_FB_2: 36,
  VIDEO_DEST_VIDEO_FB_3: 37,
  VIDEO_DEST_VIDEO_FB_4: 38,

  // Digital – Teams / BYOD mode (joins 41–45)
  TEAMS_MODE_BTN: 41,
  TEAMS_MODE_FB: 42,
  BYOD_MODE_BTN: 43,
  BYOD_MODE_FB: 44,
  BYOD_SELECT_BTN: 45,

  // Digital – source available feedback (from processor: joins 54–58)
  VIDEO_SRC_AVAIL_1: 54,
  VIDEO_SRC_AVAIL_2: 55,
  VIDEO_SRC_AVAIL_3: 56,
  VIDEO_SRC_AVAIL_4: 57,
  VIDEO_SRC_AVAIL_5: 58,

  // Digital – audio source volume-up buttons (output: joins 59–63)
  AUDIO_SRC_VOL_UP_1: 59,
  AUDIO_SRC_VOL_UP_2: 60,
  AUDIO_SRC_VOL_UP_3: 61,
  AUDIO_SRC_VOL_UP_4: 62,
  AUDIO_SRC_VOL_UP_5: 63,

  // Digital – audio source volume-down buttons (output: joins 64–68)
  AUDIO_SRC_VOL_DOWN_1: 64,
  AUDIO_SRC_VOL_DOWN_2: 65,
  AUDIO_SRC_VOL_DOWN_3: 66,
  AUDIO_SRC_VOL_DOWN_4: 67,
  AUDIO_SRC_VOL_DOWN_5: 68,

  // Digital – audio source default-volume buttons (output: joins 69–73)
  AUDIO_SRC_DEFAULT_1: 69,
  AUDIO_SRC_DEFAULT_2: 70,
  AUDIO_SRC_DEFAULT_3: 71,
  AUDIO_SRC_DEFAULT_4: 72,
  AUDIO_SRC_DEFAULT_5: 73,

  // Digital – audio source mute toggle buttons (output: joins 74–78)
  AUDIO_SRC_MUTE_BTN_1: 74,
  AUDIO_SRC_MUTE_BTN_2: 75,
  AUDIO_SRC_MUTE_BTN_3: 76,
  AUDIO_SRC_MUTE_BTN_4: 77,
  AUDIO_SRC_MUTE_BTN_5: 78,

  // Digital – audio source mute feedback (from processor: joins 79–83)
  AUDIO_SRC_MUTE_FB_1: 79,
  AUDIO_SRC_MUTE_FB_2: 80,
  AUDIO_SRC_MUTE_FB_3: 81,
  AUDIO_SRC_MUTE_FB_4: 82,
  AUDIO_SRC_MUTE_FB_5: 83,

  // Digital – far-end audio mute buttons (output: joins 84–86)
  AUDIO_PRIVACY_MUTE_BTN: 84,
  AUDIO_WIRELESS_MIC_MUTE_BTN: 85,
  AUDIO_CEILING_MIC_MUTE_BTN: 86,

  // Digital – far-end audio mute feedback (from processor: joins 87–89)
  AUDIO_PRIVACY_MUTE_FB: 87,
  AUDIO_WIRELESS_MIC_MUTE_FB: 88,
  AUDIO_CEILING_MIC_MUTE_FB: 89,

  // Digital – master volume default button (output: join 130)
  MASTER_VOL_DEFAULT_BTN: 130,

  // Digital – master volume mute button (output: join 131) and feedback (join 132)
  MASTER_VOL_MUTE_BTN: 131,
  MASTER_VOL_MUTE_FB: 132,

  // Digital – settings reset / BYOD behaviour (joins 133–137)
  SETTINGS_RESET_BTN:            133,  // panel → processor: reset UserConfig.json to defaults
  SETTINGS_BYOD_AUTO_SWITCH_BTN: 134,  // panel → processor: toggle BYOD auto-switch setting
  SETTINGS_BYOD_AUTO_SWITCH_FB:  135,  // processor → panel: BYOD auto-switch setting state
  SETTINGS_BYOD_AUTO_POWER_BTN:  136,  // panel → processor: toggle BYOD auto-power-on setting
  SETTINGS_BYOD_AUTO_POWER_FB:   137,  // processor → panel: BYOD auto-power-on setting state

  // Analog
  VOLUME_SET: 1,
  VOLUME_FB: 2,
  BRIGHTNESS_SET: 3,
  BRIGHTNESS_FB: 4,

  // Analog – audio source volume level set (joins 5–9)
  AUDIO_SRC_VOL_SET_1: 5,
  AUDIO_SRC_VOL_SET_2: 6,
  AUDIO_SRC_VOL_SET_3: 7,
  AUDIO_SRC_VOL_SET_4: 8,
  AUDIO_SRC_VOL_SET_5: 9,

  // Analog – audio source volume level feedback (from processor: joins 10–14)
  AUDIO_SRC_VOL_FB_1: 10,
  AUDIO_SRC_VOL_FB_2: 11,
  AUDIO_SRC_VOL_FB_3: 12,
  AUDIO_SRC_VOL_FB_4: 13,
  AUDIO_SRC_VOL_FB_5: 14,

  // Serial – room / header info (joins 2–6)
  DATE_SERIAL: 2,
  TIME_SERIAL: 3,
  LABEL_SERIAL: 4,
  ROOM_NAME_SERIAL: 5,
  ROOM_NUMBER_SERIAL: 6,

  // Serial – legacy source name
  SOURCE_NAME: 1,

  // Serial – video source names (joins 11–15)
  VIDEO_SRC_NAME_1: 11,
  VIDEO_SRC_NAME_2: 12,
  VIDEO_SRC_NAME_3: 13,
  VIDEO_SRC_NAME_4: 14,
  VIDEO_SRC_NAME_5: 15,

  // Serial – destination names (joins 21–24) and currently-routed source names (joins 25–28)
  VIDEO_DEST_NAME_1: 21,
  VIDEO_DEST_NAME_2: 22,
  VIDEO_DEST_NAME_3: 23,
  VIDEO_DEST_NAME_4: 24,
  VIDEO_DEST_ROUTED_SRC_NAME_1: 25,
  VIDEO_DEST_ROUTED_SRC_NAME_2: 26,
  VIDEO_DEST_ROUTED_SRC_NAME_3: 27,
  VIDEO_DEST_ROUTED_SRC_NAME_4: 28,

  // Serial – audio source names (joins 29–33)
  AUDIO_SRC_NAME_1: 29,
  AUDIO_SRC_NAME_2: 30,
  AUDIO_SRC_NAME_3: 31,
  AUDIO_SRC_NAME_4: 32,
  AUDIO_SRC_NAME_5: 33,

  // Serial – camera names (joins 34–43)
  CAM_NAME_1:  34,
  CAM_NAME_2:  35,
  CAM_NAME_3:  36,
  CAM_NAME_4:  37,
  CAM_NAME_5:  38,
  CAM_NAME_6:  39,
  CAM_NAME_7:  40,
  CAM_NAME_8:  41,
  CAM_NAME_9:  42,
  CAM_NAME_10: 43,

  // Serial – preset names for the selected camera (joins 44–53)
  CAM_PRESET_NAME_1:  44,
  CAM_PRESET_NAME_2:  45,
  CAM_PRESET_NAME_3:  46,
  CAM_PRESET_NAME_4:  47,
  CAM_PRESET_NAME_5:  48,
  CAM_PRESET_NAME_6:  49,
  CAM_PRESET_NAME_7:  50,
  CAM_PRESET_NAME_8:  51,
  CAM_PRESET_NAME_9:  52,
  CAM_PRESET_NAME_10: 53,

  // Digital – camera pan/tilt/zoom buttons (output: joins 90–95)
  CAM_PAN_LEFT_BTN: 90,
  CAM_PAN_RIGHT_BTN: 91,
  CAM_TILT_UP_BTN: 92,
  CAM_TILT_DOWN_BTN: 93,
  CAM_ZOOM_IN_BTN: 94,
  CAM_ZOOM_OUT_BTN: 95,

  // Digital – camera power / tracking toggle buttons (output: joins 96–97)
  CAM_POWER_BTN: 96,
  CAM_TRACKING_BTN: 97,

  // Digital – camera preset recall buttons (output: joins 98–107)
  CAM_PRESET_1: 98,
  CAM_PRESET_2: 99,
  CAM_PRESET_3: 100,
  CAM_PRESET_4: 101,
  CAM_PRESET_5: 102,
  CAM_PRESET_6: 103,
  CAM_PRESET_7: 104,
  CAM_PRESET_8: 105,
  CAM_PRESET_9: 106,
  CAM_PRESET_10: 107,

  // Digital – camera select buttons (output: joins 108–117)
  CAM_SELECT_1: 108,
  CAM_SELECT_2: 109,
  CAM_SELECT_3: 110,
  CAM_SELECT_4: 111,
  CAM_SELECT_5: 112,
  CAM_SELECT_6: 113,
  CAM_SELECT_7: 114,
  CAM_SELECT_8: 115,
  CAM_SELECT_9: 116,
  CAM_SELECT_10: 117,

  // Digital – camera power / tracking feedback (from processor: joins 118–119)
  CAM_POWER_FB: 118,
  CAM_TRACKING_FB: 119,

  // Digital – camera active feedback (from processor: joins 120–129)
  CAM_ACTIVE_FB_1: 120,
  CAM_ACTIVE_FB_2: 121,
  CAM_ACTIVE_FB_3: 122,
  CAM_ACTIVE_FB_4: 123,
  CAM_ACTIVE_FB_5: 124,
  CAM_ACTIVE_FB_6: 125,
  CAM_ACTIVE_FB_7: 126,
  CAM_ACTIVE_FB_8: 127,
  CAM_ACTIVE_FB_9: 128,
  CAM_ACTIVE_FB_10: 129,

  // Analog – camera zoom speed (join 15 set, join 16 feedback)
  CAM_ZOOM_SPEED_SET: 15,
  CAM_ZOOM_SPEED_FB: 16,

  // Analog – camera moment speed (join 17 set, join 18 feedback)
  CAM_MOMENT_SPEED_SET: 17,
  CAM_MOMENT_SPEED_FB: 18,

  // Analog – settings startup volume (join 19 set, join 20 feedback)
  SETTINGS_STARTUP_VOL_SET: 19,  // panel → processor: save startup volume
  SETTINGS_STARTUP_VOL_FB:  20,  // processor → panel: load startup volume from UserConfig

  // Serial – Help & Support info from SystemConfig (joins 54–56)
  HELP_IT_PHONE_SERIAL:      54,  // processor → panel: IT helpdesk phone
  HELP_SUPPORT_EMAIL_SERIAL: 55,  // processor → panel: AV support email
  HELP_QR_LABEL_SERIAL:      56,  // processor → panel: QR code label text

  // Serial – User settings, bidirectional (joins 57–60)
  SETTINGS_THEME_MODE:    57,  // "dark" | "light" | "high-contrast"
  SETTINGS_BRAND_COLOR:   58,  // hex colour string e.g. "#3b82f6"
  SETTINGS_CLOCK_FORMAT:  59,  // "12h" | "24h"
  SETTINGS_TEMP_UNIT:     60,  // "F" | "C"
} as const;

// ── Generic subscription helpers ─────────────────────────────────────────────

/**
 * Subscribe to a boolean (digital) feedback join.
 * Returns the current value and updates on every processor change.
 */
export function useDigitalJoin(joinNumber: number): boolean {
  const [value, setValue] = useState(false);

  useEffect(() => {
    const key = joinNumber.toString();
    const id = CrComLib.subscribeState('b', key, (v: boolean) => setValue(v));
    return () => {
      CrComLib.unsubscribeState('b', key, id);
    };
  }, [joinNumber]);

  return value;
}

/**
 * Subscribe to an unsigned-short (analog) feedback join.
 * Returns the current value (0–65535) and updates on every processor change.
 */
export function useAnalogJoin(joinNumber: number): number {
  const [value, setValue] = useState(0);

  useEffect(() => {
    const key = joinNumber.toString();
    const id = CrComLib.subscribeState('n', key, (v: number) => setValue(v));
    return () => {
      CrComLib.unsubscribeState('n', key, id);
    };
  }, [joinNumber]);

  return value;
}

/**
 * Subscribe to a string (serial) feedback join.
 * Returns the current string value and updates on every processor change.
 */
export function useSerialJoin(joinNumber: number): string {
  const [value, setValue] = useState('');

  useEffect(() => {
    const key = joinNumber.toString();
    const id = CrComLib.subscribeState('s', key, (v: string) => setValue(v));
    return () => {
      CrComLib.unsubscribeState('s', key, id);
    };
  }, [joinNumber]);

  return value;
}

// ── Send helpers ──────────────────────────────────────────────────────────────

/**
 * Returns a stable callback that pulses (high then low) a digital output join.
 */
export function useSendDigitalPulse(joinNumber: number): () => void {
  return useCallback(() => {
    const key = joinNumber.toString();
    CrComLib.publishEvent('b', key, true);
    // Crestron convention: release the button after 200 ms
    setTimeout(() => CrComLib.publishEvent('b', key, false), 200);
  }, [joinNumber]);
}

/**
 * Returns a stable callback that sends an analog value to an output join.
 */
export function useSendAnalog(joinNumber: number): (value: number) => void {
  return useCallback(
    (value: number) => {
      const key = joinNumber.toString();
      CrComLib.publishEvent('n', key, value);
    },
    [joinNumber],
  );
}

/**
 * Returns a stable callback that sends a string value to a serial output join.
 */
export function useSendSerial(joinNumber: number): (value: string) => void {
  return useCallback(
    (value: string) => {
      const key = joinNumber.toString();
      CrComLib.publishEvent('s', key, value);
    },
    [joinNumber],
  );
}
