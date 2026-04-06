namespace ExecutiveControlSystem
{
    /// <summary>
    /// Centralised join number constants shared by ControlSystem and RoomController.
    /// Update these values to match your SIMPL Windows / SIMPL+ signal definitions.
    /// </summary>
    public static class JoinMap
    {
        // ── Digital joins ────────────────────────────────────────────────────────

        /// <summary>Panel button – turn lights on.</summary>
        public const uint LIGHT_ON = 1;

        /// <summary>Panel button – turn lights off.</summary>
        public const uint LIGHT_OFF = 2;

        /// <summary>Feedback to panel – lights are currently on.</summary>
        public const uint LIGHT_IS_ON = 3;

        /// <summary>Panel button – toggle lights.</summary>
        public const uint LIGHT_TOGGLE = 4;

        // ── Digital – executive scenario macros (joins 5, 8–10) ─────────────────

        /// <summary>Feedback to panel – room is currently powered on (high = on, low = off).</summary>
        public const uint SYSTEM_OFF_FB = 5;

        /// <summary>Panel button – start up the room.</summary>
        public const uint SYSTEM_STARTUP_BTN = 8;

        /// <summary>Panel button – power down the room.</summary>
        public const uint SYSTEM_OFF_BTN = 9;

        /// <summary>Panel button – present local PC to all displays.</summary>
        public const uint PRESENT_TO_ROOM_BTN = 10;

        /// <summary>Panel button – select video source 1.</summary>
        public const uint VIDEO_SRC_SELECT_1 = 11;
        /// <summary>Panel button – select video source 2.</summary>
        public const uint VIDEO_SRC_SELECT_2 = 12;
        /// <summary>Panel button – select video source 3.</summary>
        public const uint VIDEO_SRC_SELECT_3 = 13;
        /// <summary>Panel button – select video source 4.</summary>
        public const uint VIDEO_SRC_SELECT_4 = 14;
        /// <summary>Panel button – select video source 5.</summary>
        public const uint VIDEO_SRC_SELECT_5 = 15;

        public static readonly uint[] VIDEO_SRC_SELECT = { VIDEO_SRC_SELECT_1, VIDEO_SRC_SELECT_2, VIDEO_SRC_SELECT_3, VIDEO_SRC_SELECT_4, VIDEO_SRC_SELECT_5 };

        /// <summary>Feedback to panel – source 1 is currently active.</summary>
        public const uint VIDEO_SRC_ACTIVE_1 = 16;
        /// <summary>Feedback to panel – source 2 is currently active.</summary>
        public const uint VIDEO_SRC_ACTIVE_2 = 17;
        /// <summary>Feedback to panel – source 3 is currently active.</summary>
        public const uint VIDEO_SRC_ACTIVE_3 = 18;
        /// <summary>Feedback to panel – source 4 is currently active.</summary>
        public const uint VIDEO_SRC_ACTIVE_4 = 19;
        /// <summary>Feedback to panel – source 5 is currently active.</summary>
        public const uint VIDEO_SRC_ACTIVE_5 = 20;

        /// <summary>Panel button – toggle power for destination 1.</summary>
        public const uint VIDEO_DEST_POWER_BTN_1 = 21;
        /// <summary>Panel button – toggle power for destination 2.</summary>
        public const uint VIDEO_DEST_POWER_BTN_2 = 22;
        /// <summary>Panel button – toggle power for destination 3.</summary>
        public const uint VIDEO_DEST_POWER_BTN_3 = 23;
        /// <summary>Panel button – toggle power for destination 4.</summary>
        public const uint VIDEO_DEST_POWER_BTN_4 = 24;

        /// <summary>Feedback to panel – destination 1 power is on.</summary>
        public const uint VIDEO_DEST_POWER_FB_1 = 25;
        /// <summary>Feedback to panel – destination 2 power is on.</summary>
        public const uint VIDEO_DEST_POWER_FB_2 = 26;
        /// <summary>Feedback to panel – destination 3 power is on.</summary>
        public const uint VIDEO_DEST_POWER_FB_3 = 27;
        /// <summary>Feedback to panel – destination 4 power is on.</summary>
        public const uint VIDEO_DEST_POWER_FB_4 = 28;

        /// <summary>Panel button – toggle video output for destination 1.</summary>
        public const uint VIDEO_DEST_VIDEO_BTN_1 = 31;
        /// <summary>Panel button – toggle video output for destination 2.</summary>
        public const uint VIDEO_DEST_VIDEO_BTN_2 = 32;
        /// <summary>Panel button – toggle video output for destination 3.</summary>
        public const uint VIDEO_DEST_VIDEO_BTN_3 = 33;
        /// <summary>Panel button – toggle video output for destination 4.</summary>
        public const uint VIDEO_DEST_VIDEO_BTN_4 = 34;

        /// <summary>Feedback to panel – destination 1 video output is on.</summary>
        public const uint VIDEO_DEST_VIDEO_FB_1 = 35;
        /// <summary>Feedback to panel – destination 2 video output is on.</summary>
        public const uint VIDEO_DEST_VIDEO_FB_2 = 36;
        /// <summary>Feedback to panel – destination 3 video output is on.</summary>
        public const uint VIDEO_DEST_VIDEO_FB_3 = 37;
        /// <summary>Feedback to panel – destination 4 video output is on.</summary>
        public const uint VIDEO_DEST_VIDEO_FB_4 = 38;

        /// <summary>Panel button – toggle Ingest Mode.</summary>
        public const uint INGEST_MODE_BTN = 41;
        /// <summary>Feedback to panel – Ingest Mode is active.</summary>
        public const uint INGEST_MODE_FB = 42;
        /// <summary>Panel button – toggle BYOD mode.</summary>
        public const uint BYOD_MODE_BTN = 43;
        /// <summary>Feedback to panel – BYOD mode is active.</summary>
        public const uint BYOD_MODE_FB = 44;
        /// <summary>Panel button – BYOD device select.</summary>
        public const uint BYOD_SELECT_BTN = 45;

        /// <summary>Panel button – select destination 1 for routing.</summary>
        public const uint VIDEO_DEST_SELECT_1 = 46;
        /// <summary>Panel button – select destination 2 for routing.</summary>
        public const uint VIDEO_DEST_SELECT_2 = 47;
        /// <summary>Panel button – select destination 3 for routing.</summary>
        public const uint VIDEO_DEST_SELECT_3 = 48;
        /// <summary>Panel button – select destination 4 for routing.</summary>
        public const uint VIDEO_DEST_SELECT_4 = 49;

        public static readonly uint[] VIDEO_DEST_SELECT = { VIDEO_DEST_SELECT_1, VIDEO_DEST_SELECT_2, VIDEO_DEST_SELECT_3, VIDEO_DEST_SELECT_4 };

        /// <summary>Feedback to panel – destination 1 is selected for routing.</summary>
        public const uint VIDEO_DEST_ACTIVE_1 = 50;
        /// <summary>Feedback to panel – destination 2 is selected for routing.</summary>
        public const uint VIDEO_DEST_ACTIVE_2 = 51;
        /// <summary>Feedback to panel – destination 3 is selected for routing.</summary>
        public const uint VIDEO_DEST_ACTIVE_3 = 52;
        /// <summary>Feedback to panel – destination 4 is selected for routing.</summary>
        public const uint VIDEO_DEST_ACTIVE_4 = 53;

        /// <summary>Feedback to panel – source 1 signal is present (source available).</summary>
        public const uint VIDEO_SRC_AVAIL_1 = 54;
        /// <summary>Feedback to panel – source 2 signal is present (source available).</summary>
        public const uint VIDEO_SRC_AVAIL_2 = 55;
        /// <summary>Feedback to panel – source 3 signal is present (source available).</summary>
        public const uint VIDEO_SRC_AVAIL_3 = 56;
        /// <summary>Feedback to panel – source 4 signal is present (source available).</summary>
        public const uint VIDEO_SRC_AVAIL_4 = 57;
        /// <summary>Feedback to panel – source 5 signal is present (source available).</summary>
        public const uint VIDEO_SRC_AVAIL_5 = 58;

        // ── Digital – audio source volume-up buttons (output: joins 59–63) ────────
        /// <summary>Panel button (hold) – increase volume for audio source 1.</summary>
        public const uint AUDIO_SRC_VOL_UP_1 = 59;
        /// <summary>Panel button (hold) – increase volume for audio source 2.</summary>
        public const uint AUDIO_SRC_VOL_UP_2 = 60;
        /// <summary>Panel button (hold) – increase volume for audio source 3.</summary>
        public const uint AUDIO_SRC_VOL_UP_3 = 61;
        /// <summary>Panel button (hold) – increase volume for audio source 4.</summary>
        public const uint AUDIO_SRC_VOL_UP_4 = 62;
        /// <summary>Panel button (hold) – increase volume for audio source 5.</summary>
        public const uint AUDIO_SRC_VOL_UP_5 = 63;

        // ── Digital – audio source volume-down buttons (output: joins 64–68) ──────
        /// <summary>Panel button (hold) – decrease volume for audio source 1.</summary>
        public const uint AUDIO_SRC_VOL_DOWN_1 = 64;
        /// <summary>Panel button (hold) – decrease volume for audio source 2.</summary>
        public const uint AUDIO_SRC_VOL_DOWN_2 = 65;
        /// <summary>Panel button (hold) – decrease volume for audio source 3.</summary>
        public const uint AUDIO_SRC_VOL_DOWN_3 = 66;
        /// <summary>Panel button (hold) – decrease volume for audio source 4.</summary>
        public const uint AUDIO_SRC_VOL_DOWN_4 = 67;
        /// <summary>Panel button (hold) – decrease volume for audio source 5.</summary>
        public const uint AUDIO_SRC_VOL_DOWN_5 = 68;

        // ── Digital – audio source default-volume buttons (output: joins 69–73) ───
        /// <summary>Panel button – restore default volume for audio source 1.</summary>
        public const uint AUDIO_SRC_DEFAULT_1 = 69;
        /// <summary>Panel button – restore default volume for audio source 2.</summary>
        public const uint AUDIO_SRC_DEFAULT_2 = 70;
        /// <summary>Panel button – restore default volume for audio source 3.</summary>
        public const uint AUDIO_SRC_DEFAULT_3 = 71;
        /// <summary>Panel button – restore default volume for audio source 4.</summary>
        public const uint AUDIO_SRC_DEFAULT_4 = 72;
        /// <summary>Panel button – restore default volume for audio source 5.</summary>
        public const uint AUDIO_SRC_DEFAULT_5 = 73;

        // ── Digital – audio source mute toggle buttons (output: joins 74–78) ──────
        /// <summary>Panel button – toggle mute for audio source 1.</summary>
        public const uint AUDIO_SRC_MUTE_BTN_1 = 74;
        /// <summary>Panel button – toggle mute for audio source 2.</summary>
        public const uint AUDIO_SRC_MUTE_BTN_2 = 75;
        /// <summary>Panel button – toggle mute for audio source 3.</summary>
        public const uint AUDIO_SRC_MUTE_BTN_3 = 76;
        /// <summary>Panel button – toggle mute for audio source 4.</summary>
        public const uint AUDIO_SRC_MUTE_BTN_4 = 77;
        /// <summary>Panel button – toggle mute for audio source 5.</summary>
        public const uint AUDIO_SRC_MUTE_BTN_5 = 78;

        // ── Digital – audio source mute feedback (from processor: joins 79–83) ────
        /// <summary>Feedback to panel – audio source 1 is muted.</summary>
        public const uint AUDIO_SRC_MUTE_FB_1 = 79;
        /// <summary>Feedback to panel – audio source 2 is muted.</summary>
        public const uint AUDIO_SRC_MUTE_FB_2 = 80;
        /// <summary>Feedback to panel – audio source 3 is muted.</summary>
        public const uint AUDIO_SRC_MUTE_FB_3 = 81;
        /// <summary>Feedback to panel – audio source 4 is muted.</summary>
        public const uint AUDIO_SRC_MUTE_FB_4 = 82;
        /// <summary>Feedback to panel – audio source 5 is muted.</summary>
        public const uint AUDIO_SRC_MUTE_FB_5 = 83;

        // ── Digital – far-end audio mute buttons (output: joins 84–86) ───────────
        /// <summary>Panel button – toggle privacy mute.</summary>
        public const uint AUDIO_PRIVACY_MUTE_BTN = 84;
        /// <summary>Panel button – toggle wireless microphone mute.</summary>
        public const uint AUDIO_WIRELESS_MIC_MUTE_BTN = 85;
        /// <summary>Panel button – toggle ceiling microphone mute.</summary>
        public const uint AUDIO_CEILING_MIC_MUTE_BTN = 86;

        // ── Digital – far-end audio mute feedback (from processor: joins 87–89) ───
        /// <summary>Feedback to panel – privacy mute is active.</summary>
        public const uint AUDIO_PRIVACY_MUTE_FB = 87;
        /// <summary>Feedback to panel – wireless microphone mute is active.</summary>
        public const uint AUDIO_WIRELESS_MIC_MUTE_FB = 88;
        /// <summary>Feedback to panel – ceiling microphone mute is active.</summary>
        public const uint AUDIO_CEILING_MIC_MUTE_FB = 89;

        // ── Digital – camera pan / tilt / zoom buttons (output: joins 90–95) ──────
        /// <summary>Panel button (hold) – pan camera left.</summary>
        public const uint CAM_PAN_LEFT_BTN = 90;
        /// <summary>Panel button (hold) – pan camera right.</summary>
        public const uint CAM_PAN_RIGHT_BTN = 91;
        /// <summary>Panel button (hold) – tilt camera up.</summary>
        public const uint CAM_TILT_UP_BTN = 92;
        /// <summary>Panel button (hold) – tilt camera down.</summary>
        public const uint CAM_TILT_DOWN_BTN = 93;
        /// <summary>Panel button (hold) – zoom camera in.</summary>
        public const uint CAM_ZOOM_IN_BTN = 94;
        /// <summary>Panel button (hold) – zoom camera out.</summary>
        public const uint CAM_ZOOM_OUT_BTN = 95;

        // ── Digital – camera power / tracking toggle buttons (output: joins 96–97) ─
        /// <summary>Panel button – toggle camera power.</summary>
        public const uint CAM_POWER_BTN = 96;
        /// <summary>Panel button – toggle camera auto-tracking.</summary>
        public const uint CAM_TRACKING_BTN = 97;

        // ── Digital – camera preset recall buttons (output: joins 98–107) ──────────
        /// <summary>Panel button – recall camera preset 1.</summary>
        public const uint CAM_PRESET_1 = 98;
        /// <summary>Panel button – recall camera preset 2.</summary>
        public const uint CAM_PRESET_2 = 99;
        /// <summary>Panel button – recall camera preset 3.</summary>
        public const uint CAM_PRESET_3 = 100;
        /// <summary>Panel button – recall camera preset 4.</summary>
        public const uint CAM_PRESET_4 = 101;
        /// <summary>Panel button – recall camera preset 5.</summary>
        public const uint CAM_PRESET_5 = 102;
        /// <summary>Panel button – recall camera preset 6.</summary>
        public const uint CAM_PRESET_6 = 103;
        /// <summary>Panel button – recall camera preset 7.</summary>
        public const uint CAM_PRESET_7 = 104;
        /// <summary>Panel button – recall camera preset 8.</summary>
        public const uint CAM_PRESET_8 = 105;
        /// <summary>Panel button – recall camera preset 9.</summary>
        public const uint CAM_PRESET_9 = 106;
        /// <summary>Panel button – recall camera preset 10.</summary>
        public const uint CAM_PRESET_10 = 107;

        // ── Digital – camera select buttons (output: joins 108–117) ─────────────────
        /// <summary>Panel button – select camera 1.</summary>
        public const uint CAM_SELECT_1 = 108;
        /// <summary>Panel button – select camera 2.</summary>
        public const uint CAM_SELECT_2 = 109;
        /// <summary>Panel button – select camera 3.</summary>
        public const uint CAM_SELECT_3 = 110;
        /// <summary>Panel button – select camera 4.</summary>
        public const uint CAM_SELECT_4 = 111;
        /// <summary>Panel button – select camera 5.</summary>
        public const uint CAM_SELECT_5 = 112;
        /// <summary>Panel button – select camera 6.</summary>
        public const uint CAM_SELECT_6 = 113;
        /// <summary>Panel button – select camera 7.</summary>
        public const uint CAM_SELECT_7 = 114;
        /// <summary>Panel button – select camera 8.</summary>
        public const uint CAM_SELECT_8 = 115;
        /// <summary>Panel button – select camera 9.</summary>
        public const uint CAM_SELECT_9 = 116;
        /// <summary>Panel button – select camera 10.</summary>
        public const uint CAM_SELECT_10 = 117;

        // ── Digital – camera power / tracking feedback (from processor: joins 118–119) ─
        /// <summary>Feedback to panel – camera power is on.</summary>
        public const uint CAM_POWER_FB = 118;
        /// <summary>Feedback to panel – camera auto-tracking is active.</summary>
        public const uint CAM_TRACKING_FB = 119;

        // ── Digital – camera active feedback (from processor: joins 120–129) ─────────
        /// <summary>Feedback to panel – camera 1 is selected.</summary>
        public const uint CAM_ACTIVE_FB_1 = 120;
        /// <summary>Feedback to panel – camera 2 is selected.</summary>
        public const uint CAM_ACTIVE_FB_2 = 121;
        /// <summary>Feedback to panel – camera 3 is selected.</summary>
        public const uint CAM_ACTIVE_FB_3 = 122;
        /// <summary>Feedback to panel – camera 4 is selected.</summary>
        public const uint CAM_ACTIVE_FB_4 = 123;
        /// <summary>Feedback to panel – camera 5 is selected.</summary>
        public const uint CAM_ACTIVE_FB_5 = 124;
        /// <summary>Feedback to panel – camera 6 is selected.</summary>
        public const uint CAM_ACTIVE_FB_6 = 125;
        /// <summary>Feedback to panel – camera 7 is selected.</summary>
        public const uint CAM_ACTIVE_FB_7 = 126;
        /// <summary>Feedback to panel – camera 8 is selected.</summary>
        public const uint CAM_ACTIVE_FB_8 = 127;
        /// <summary>Feedback to panel – camera 9 is selected.</summary>
        public const uint CAM_ACTIVE_FB_9 = 128;
        /// <summary>Feedback to panel – camera 10 is selected.</summary>
        public const uint CAM_ACTIVE_FB_10 = 129;

        // ── Digital – master volume default / mute (joins 130–132) ───────────────
        /// <summary>Button from panel – restore master volume to default level.</summary>
        public const uint MASTER_VOL_DEFAULT_BTN = 130;

        /// <summary>Button from panel – toggle master volume mute on/off.</summary>
        public const uint MASTER_VOL_MUTE_BTN = 131;

        /// <summary>Feedback to panel – master volume is currently muted.</summary>
        public const uint MASTER_VOL_MUTE_FB = 132;

        // ── Analog joins ─────────────────────────────────────────────────────────
        /// <summary>Panel slider – set master volume level (0–65 535).</summary>
        public const uint VOLUME_SET = 1;

        /// <summary>Feedback to panel – current master volume level (0–65 535).</summary>
        public const uint VOLUME_FB = 2;

        /// <summary>Panel slider – set display brightness (0–65 535).</summary>
        public const uint BRIGHTNESS_SET = 3;

        /// <summary>Feedback to panel – current display brightness (0–65 535).</summary>
        public const uint BRIGHTNESS_FB = 4;

        // ── Analog – audio source volume level set (joins 5–9) ───────────────────
        /// <summary>Panel slider – set volume level for audio source 1 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_SET_1 = 5;
        /// <summary>Panel slider – set volume level for audio source 2 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_SET_2 = 6;
        /// <summary>Panel slider – set volume level for audio source 3 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_SET_3 = 7;
        /// <summary>Panel slider – set volume level for audio source 4 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_SET_4 = 8;
        /// <summary>Panel slider – set volume level for audio source 5 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_SET_5 = 9;

        // ── Analog – audio source volume level feedback (from processor: joins 10–14) ──
        /// <summary>Feedback to panel – current volume level for audio source 1 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_FB_1 = 10;
        /// <summary>Feedback to panel – current volume level for audio source 2 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_FB_2 = 11;
        /// <summary>Feedback to panel – current volume level for audio source 3 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_FB_3 = 12;
        /// <summary>Feedback to panel – current volume level for audio source 4 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_FB_4 = 13;
        /// <summary>Feedback to panel – current volume level for audio source 5 (0–65 535).</summary>
        public const uint AUDIO_SRC_VOL_FB_5 = 14;

        // ── Analog – camera zoom speed (join 15 set, join 16 feedback) ──────────────
        /// <summary>Panel slider – set camera zoom speed (0–65 535).</summary>
        public const uint CAM_ZOOM_SPEED_SET = 15;
        /// <summary>Feedback to panel – current camera zoom speed (0–65 535).</summary>
        public const uint CAM_ZOOM_SPEED_FB = 16;

        // ── Analog – camera moment speed (join 17 set, join 18 feedback) ────────────
        /// <summary>Panel slider – set camera moment (pan/tilt) speed (0–65 535).</summary>
        public const uint CAM_MOMENT_SPEED_SET = 17;
        /// <summary>Feedback to panel – current camera moment speed (0–65 535).</summary>
        public const uint CAM_MOMENT_SPEED_FB = 18;

        // ── Serial joins ─────────────────────────────────────────────────────────
        /// <summary>Feedback to panel – legacy single source name.</summary>
        public const uint SOURCE_NAME = 1;

        /// <summary>Feedback to panel – current date string.</summary>
        public const uint DATE_SERIAL = 2;
        /// <summary>Feedback to panel – current time string.</summary>
        public const uint TIME_SERIAL = 3;
        /// <summary>Feedback to panel – panel label / system label.</summary>
        public const uint LABEL_SERIAL = 4;
        /// <summary>Feedback to panel – room name.</summary>
        public const uint ROOM_NAME_SERIAL = 5;
        /// <summary>Feedback to panel – room number.</summary>
        public const uint ROOM_NUMBER_SERIAL = 6;

        /// <summary>Feedback to panel – name of video source 1 from the control system.</summary>
        public const uint VIDEO_SRC_NAME_1 = 11;
        /// <summary>Feedback to panel – name of video source 2 from the control system.</summary>
        public const uint VIDEO_SRC_NAME_2 = 12;
        /// <summary>Feedback to panel – name of video source 3 from the control system.</summary>
        public const uint VIDEO_SRC_NAME_3 = 13;
        /// <summary>Feedback to panel – name of video source 4 from the control system.</summary>
        public const uint VIDEO_SRC_NAME_4 = 14;
        /// <summary>Feedback to panel – name of video source 5 from the control system.</summary>
        public const uint VIDEO_SRC_NAME_5 = 15;

        /// <summary>Feedback to panel – display name of destination 1.</summary>
        public const uint VIDEO_DEST_NAME_1 = 21;
        /// <summary>Feedback to panel – display name of destination 2.</summary>
        public const uint VIDEO_DEST_NAME_2 = 22;
        /// <summary>Feedback to panel – display name of destination 3.</summary>
        public const uint VIDEO_DEST_NAME_3 = 23;
        /// <summary>Feedback to panel – display name of destination 4.</summary>
        public const uint VIDEO_DEST_NAME_4 = 24;

        /// <summary>Feedback to panel – name of the source currently routed to destination 1.</summary>
        public const uint VIDEO_DEST_ROUTED_SRC_NAME_1 = 25;
        /// <summary>Feedback to panel – name of the source currently routed to destination 2.</summary>
        public const uint VIDEO_DEST_ROUTED_SRC_NAME_2 = 26;
        /// <summary>Feedback to panel – name of the source currently routed to destination 3.</summary>
        public const uint VIDEO_DEST_ROUTED_SRC_NAME_3 = 27;
        /// <summary>Feedback to panel – name of the source currently routed to destination 4.</summary>
        public const uint VIDEO_DEST_ROUTED_SRC_NAME_4 = 28;

        public static readonly uint[] VIDEO_DEST_ROUTED_SRC_NAME = { VIDEO_DEST_ROUTED_SRC_NAME_1, VIDEO_DEST_ROUTED_SRC_NAME_2, VIDEO_DEST_ROUTED_SRC_NAME_3, VIDEO_DEST_ROUTED_SRC_NAME_4 };

        // ── Serial – audio source names (joins 29–33) ────────────────────────────
        /// <summary>Feedback to panel – display name of audio source 1.</summary>
        public const uint AUDIO_SRC_NAME_1 = 29;
        /// <summary>Feedback to panel – display name of audio source 2.</summary>
        public const uint AUDIO_SRC_NAME_2 = 30;
        /// <summary>Feedback to panel – display name of audio source 3.</summary>
        public const uint AUDIO_SRC_NAME_3 = 31;
        /// <summary>Feedback to panel – display name of audio source 4.</summary>
        public const uint AUDIO_SRC_NAME_4 = 32;
        /// <summary>Feedback to panel – display name of audio source 5.</summary>
        public const uint AUDIO_SRC_NAME_5 = 33;

        // ── Serial – camera names (joins 34–43) ─────────────────────────────────
        /// <summary>Feedback to panel – display name of camera 1.</summary>
        public const uint CAM_NAME_1  = 34;
        /// <summary>Feedback to panel – display name of camera 2.</summary>
        public const uint CAM_NAME_2  = 35;
        /// <summary>Feedback to panel – display name of camera 3.</summary>
        public const uint CAM_NAME_3  = 36;
        /// <summary>Feedback to panel – display name of camera 4.</summary>
        public const uint CAM_NAME_4  = 37;
        /// <summary>Feedback to panel – display name of camera 5.</summary>
        public const uint CAM_NAME_5  = 38;
        /// <summary>Feedback to panel – display name of camera 6.</summary>
        public const uint CAM_NAME_6  = 39;
        /// <summary>Feedback to panel – display name of camera 7.</summary>
        public const uint CAM_NAME_7  = 40;
        /// <summary>Feedback to panel – display name of camera 8.</summary>
        public const uint CAM_NAME_8  = 41;
        /// <summary>Feedback to panel – display name of camera 9.</summary>
        public const uint CAM_NAME_9  = 42;
        /// <summary>Feedback to panel – display name of camera 10.</summary>
        public const uint CAM_NAME_10 = 43;

        /// <summary>All camera name serial joins in order (index 0 = camera 1).</summary>
        public static readonly uint[] CAM_NAME_SERIALS = {
            CAM_NAME_1, CAM_NAME_2, CAM_NAME_3, CAM_NAME_4, CAM_NAME_5,
            CAM_NAME_6, CAM_NAME_7, CAM_NAME_8, CAM_NAME_9, CAM_NAME_10,
        };

        // ── Serial – camera preset names (joins 44–53) ───────────────────────────
        /// <summary>Feedback to panel – name of preset 1 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_1  = 44;
        /// <summary>Feedback to panel – name of preset 2 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_2  = 45;
        /// <summary>Feedback to panel – name of preset 3 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_3  = 46;
        /// <summary>Feedback to panel – name of preset 4 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_4  = 47;
        /// <summary>Feedback to panel – name of preset 5 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_5  = 48;
        /// <summary>Feedback to panel – name of preset 6 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_6  = 49;
        /// <summary>Feedback to panel – name of preset 7 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_7  = 50;
        /// <summary>Feedback to panel – name of preset 8 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_8  = 51;
        /// <summary>Feedback to panel – name of preset 9 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_9  = 52;
        /// <summary>Feedback to panel – name of preset 10 for the selected camera.</summary>
        public const uint CAM_PRESET_NAME_10 = 53;

        /// <summary>All camera preset name serial joins in order (index 0 = preset 1).</summary>
        public static readonly uint[] CAM_PRESET_NAME_SERIALS = {
            CAM_PRESET_NAME_1, CAM_PRESET_NAME_2, CAM_PRESET_NAME_3, CAM_PRESET_NAME_4, CAM_PRESET_NAME_5,
            CAM_PRESET_NAME_6, CAM_PRESET_NAME_7, CAM_PRESET_NAME_8, CAM_PRESET_NAME_9, CAM_PRESET_NAME_10,
        };

        // ── Serial – Help & Support info from SystemConfig (joins 54–56) ────────
        /// <summary>Feedback to panel – IT helpdesk phone number (from SystemConfig).</summary>
        public const uint HELP_IT_PHONE_SERIAL = 54;
        /// <summary>Feedback to panel – AV support email address (from SystemConfig).</summary>
        public const uint HELP_SUPPORT_EMAIL_SERIAL = 55;
        /// <summary>Feedback to panel – QR code label text (from SystemConfig).</summary>
        public const uint HELP_QR_LABEL_SERIAL = 56;

        // ── Serial – Network info from control system (joins 61–64) ─────────────
        /// <summary>Feedback to panel – control system LAN adapter IP address.</summary>
        public const uint NETWORK_PANEL_IP_SERIAL = 61;
        /// <summary>Feedback to panel – control system LAN adapter subnet mask.</summary>
        public const uint NETWORK_SUBNET_SERIAL = 62;
        /// <summary>Feedback to panel – control system LAN adapter MAC address.</summary>
        public const uint NETWORK_MAC_SERIAL = 63;
        /// <summary>Feedback to panel – control system IP address (same as Panel IP; both reflect the LAN-A adapter address).</summary>
        public const uint NETWORK_CS_IP_SERIAL = 64;

        // ── Serial – User settings (joins 57–60, bidirectional) ─────────────────
        /// <summary>Bidirectional – UI theme mode ("dark" / "light" / "high-contrast").</summary>
        public const uint SETTINGS_THEME_MODE = 57;
        /// <summary>Bidirectional – brand accent colour hex string (e.g. "#3b82f6").</summary>
        public const uint SETTINGS_BRAND_COLOR = 58;
        /// <summary>Bidirectional – clock format ("12h" / "24h").</summary>
        public const uint SETTINGS_CLOCK_FORMAT = 59;
        /// <summary>Bidirectional – temperature unit ("F" / "C").</summary>
        public const uint SETTINGS_TEMP_UNIT = 60;

        // ── Analog – settings startup volume (joins 19–20) ───────────────────────
        /// <summary>Panel sends startup volume level (0–65535) to processor for persistence.</summary>
        public const uint SETTINGS_STARTUP_VOL_SET = 19;
        /// <summary>Feedback to panel – startup volume level (0–65535) from UserConfig.</summary>
        public const uint SETTINGS_STARTUP_VOL_FB = 20;

        // ── Digital – settings reset / BYOD behaviour (joins 133–137) ───────────
        /// <summary>Panel button – reset all UserConfig.json values to defaults.</summary>
        public const uint SETTINGS_RESET_BTN = 133;
        /// <summary>Panel button – toggle BYOD auto-switch-on-connect setting.</summary>
        public const uint SETTINGS_BYOD_AUTO_SWITCH_BTN = 134;
        /// <summary>Feedback to panel – BYOD auto-switch setting is enabled.</summary>
        public const uint SETTINGS_BYOD_AUTO_SWITCH_FB = 135;
        /// <summary>Panel button – toggle BYOD auto-power-on setting.</summary>
        public const uint SETTINGS_BYOD_AUTO_POWER_BTN = 136;
        /// <summary>Feedback to panel – BYOD auto-power-on setting is enabled.</summary>
        public const uint SETTINGS_BYOD_AUTO_POWER_FB = 137;

        // ── Digital – source visible in "far end" section during Ingest Mode (joins 138–142) ──
        /// <summary>Feedback to panel – source 1 is visible in the "far end" section (non-Room-PC, Ingest Mode active).</summary>
        public const uint VIDEO_SRC_FAR_END_VISIBLE_1 = 138;
        /// <summary>Feedback to panel – source 2 is visible in the "far end" section (non-Room-PC, Ingest Mode active).</summary>
        public const uint VIDEO_SRC_FAR_END_VISIBLE_2 = 139;
        /// <summary>Feedback to panel – source 3 is visible in the "far end" section (non-Room-PC, Ingest Mode active).</summary>
        public const uint VIDEO_SRC_FAR_END_VISIBLE_3 = 140;
        /// <summary>Feedback to panel – source 4 is visible in the "far end" section (non-Room-PC, Ingest Mode active).</summary>
        public const uint VIDEO_SRC_FAR_END_VISIBLE_4 = 141;
        /// <summary>Feedback to panel – source 5 is visible in the "far end" section (non-Room-PC, Ingest Mode active).</summary>
        public const uint VIDEO_SRC_FAR_END_VISIBLE_5 = 142;

        public static readonly uint[] VIDEO_SRC_FAR_END_VISIBLE = {
            VIDEO_SRC_FAR_END_VISIBLE_1, VIDEO_SRC_FAR_END_VISIBLE_2,
            VIDEO_SRC_FAR_END_VISIBLE_3, VIDEO_SRC_FAR_END_VISIBLE_4,
            VIDEO_SRC_FAR_END_VISIBLE_5,
        };
    }
}
