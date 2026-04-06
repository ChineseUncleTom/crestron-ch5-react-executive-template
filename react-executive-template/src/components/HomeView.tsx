import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Tv2, Video, PowerOff, Power, Mic, MicOff } from 'lucide-react';
import {
  Joins,
  useDigitalJoin,
  useSerialJoin,
  useSendDigitalPulse,
  useAnalogJoin,
  useSendAnalog,
} from '../hooks/useCrestron';

/**
 * HomeView
 *
 * Executive landing page with one-touch scenario macros and a live status
 * summary. Designed for zero-training usage – executives can start, switch,
 * or end a room session in a single tap.
 *
 * Scenario buttons:
 * - "Present to Room"  – pulses PRESENT_TO_ROOM_BTN (join 10); the control
 *   system routes the local PC to all displays and powers them on.
 * - "Video Conference" – status indicator driven by ROOM_PC_IN_MEETING_FB
 *   (join 143); shows "Room in meeting" when high, "Room is ready" when low.
 * - "System Off/On"    – when room is on (SYSTEM_OFF_FB high): hold for 5 s
 *   to pulse SYSTEM_OFF_BTN (join 9). When room is off (SYSTEM_OFF_FB low):
 *   single press pulses SYSTEM_STARTUP_BTN (join 8).
 */
const HOLD_DURATION_MS = 5000;

const HomeView: React.FC = () => {
  const roomName        = useSerialJoin(Joins.ROOM_NAME_SERIAL);
  const roomPCInMeeting = useDigitalJoin(Joins.ROOM_PC_IN_MEETING_FB);
  const byodActive      = useDigitalJoin(Joins.BYOD_MODE_FB);
  const privacyMuted    = useDigitalJoin(Joins.AUDIO_PRIVACY_MUTE_FB);
  const masterVolFb     = useAnalogJoin(Joins.VOLUME_FB);
  // HIGH = room is currently powered on; LOW = room is off
  const roomIsOn = useDigitalJoin(Joins.SYSTEM_OFF_FB);

  const sendPresentToRoom = useSendDigitalPulse(Joins.PRESENT_TO_ROOM_BTN);
  const sendSystemOff     = useSendDigitalPulse(Joins.SYSTEM_OFF_BTN);
  const sendSystemStartup = useSendDigitalPulse(Joins.SYSTEM_STARTUP_BTN);
  const sendPrivacyMute   = useSendDigitalPulse(Joins.AUDIO_PRIVACY_MUTE_BTN);

  const sendMasterVol = useSendAnalog(Joins.VOLUME_SET);
  const masterVolPct = Math.round((masterVolFb / 65535) * 100);

  // ── Master Volume card – swipe-to-control ───────────────────────────────
  // Keep a ref so pointer callbacks don't capture a stale value.
  const masterVolFbRef = useRef(masterVolFb);
  masterVolFbRef.current = masterVolFb;

  const [isDraggingVol, setIsDraggingVol] = useState(false);
  const isDraggingVolRef = useRef(false);
  const volDragStartXRef = useRef(0);
  const volDragStartValRef = useRef(0);
  const volCardWidthRef = useRef(1);

  const handleVolPointerDown = useCallback((e: React.PointerEvent<HTMLDivElement>) => {
    e.currentTarget.setPointerCapture(e.pointerId);
    volDragStartXRef.current = e.clientX;
    volDragStartValRef.current = masterVolFbRef.current;
    volCardWidthRef.current = e.currentTarget.offsetWidth || 1;
    isDraggingVolRef.current = true;
    setIsDraggingVol(true);
  }, []);

  const handleVolPointerMove = useCallback((e: React.PointerEvent<HTMLDivElement>) => {
    if (!isDraggingVolRef.current) return;
    const deltaX = e.clientX - volDragStartXRef.current;
    const deltaRaw = Math.round((deltaX / volCardWidthRef.current) * 65535);
    const newVol = Math.max(0, Math.min(65535, volDragStartValRef.current + deltaRaw));
    sendMasterVol(newVol);
  }, [sendMasterVol]);

  const stopVolDrag = useCallback((e: React.PointerEvent<HTMLDivElement>) => {
    if (!isDraggingVolRef.current) return;
    e.currentTarget.releasePointerCapture(e.pointerId);
    isDraggingVolRef.current = false;
    setIsDraggingVol(false);
  }, []);

  // ── Hold-to-power-off logic ─────────────────────────────────────────────
  const [holdProgress, setHoldProgress] = useState(0);
  const holdIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const cancelHold = useCallback(() => {
    if (holdIntervalRef.current !== null) {
      clearInterval(holdIntervalRef.current);
      holdIntervalRef.current = null;
    }
    setHoldProgress(0);
  }, []);

  const startHold = useCallback(() => {
    if (holdIntervalRef.current !== null) return;
    const startTime = Date.now();
    holdIntervalRef.current = setInterval(() => {
      const pct = Math.min(((Date.now() - startTime) / HOLD_DURATION_MS) * 100, 100);
      setHoldProgress(pct);
      if (pct >= 100) {
        clearInterval(holdIntervalRef.current!);
        holdIntervalRef.current = null;
        setHoldProgress(0);
        sendSystemOff();
      }
    }, 50);
  }, [sendSystemOff]);

  // Cancel hold if the room state changes while pressing
  useEffect(() => {
    if (!roomIsOn) cancelHold();
  }, [roomIsOn, cancelHold]);

  return (
    <div className="home-view">

      {/* ── Room status banner ───────────────────────────────────────────── */}
      <div className="home-view__status-banner">
        <span className="home-view__room-name">{roomName || 'Executive Boardroom'}</span>
        <span className={`home-view__status-pill${roomPCInMeeting ? ' home-view__status-pill--meeting' : byodActive ? ' home-view__status-pill--byod' : ' home-view__status-pill--idle'}`}>
          {roomPCInMeeting ? 'In Meeting' : byodActive ? 'BYOD Active' : 'Room Ready'}
        </span>
      </div>

      {/* ── Scenario macro buttons ───────────────────────────────────────── */}
      <section className="home-view__scenarios" aria-label="Room scenarios">
        <button
          className="home-view__scenario-btn home-view__scenario-btn--present"
          onClick={sendPresentToRoom}
          aria-label="Present to Room"
        >
          <span className="home-view__scenario-icon" aria-hidden="true">
            <Tv2 size={40} />
          </span>
          <span className="home-view__scenario-label">Present to Room</span>
          <span className="home-view__scenario-sub">Route local PC to all displays</span>
        </button>

        <div
          className={`home-view__scenario-btn home-view__scenario-btn--video${roomPCInMeeting ? ' home-view__scenario-btn--active' : ''}`}
          aria-label="Video Conference"
          role="status"
          aria-live="polite"
        >
          <span className="home-view__scenario-icon" aria-hidden="true">
            <Video size={40} />
          </span>
          <span className="home-view__scenario-label">Video Conference</span>
          <span className="home-view__scenario-sub">{roomPCInMeeting ? 'Room in meeting' : 'Room is ready'}</span>
        </div>

        <button
          className={[
            'home-view__scenario-btn',
            roomIsOn ? 'home-view__scenario-btn--power-off' : 'home-view__scenario-btn--power-on',
          ].join(' ')}
          {...(roomIsOn
            ? {
                onPointerDown: startHold,
                onPointerUp: cancelHold,
                onPointerLeave: cancelHold,
                onPointerCancel: cancelHold,
              }
            : {
                onClick: sendSystemStartup,
              })}
          aria-label={roomIsOn ? 'System Off – hold to power down' : 'System On – press to power on'}
        >
          {roomIsOn && holdProgress > 0 && (
            <span
              className="home-view__hold-progress"
              style={{ width: `${holdProgress}%` }}
              aria-hidden="true"
            />
          )}
          <span className="home-view__scenario-icon" aria-hidden="true">
            {roomIsOn ? <PowerOff size={40} /> : <Power size={40} />}
          </span>
          <span className="home-view__scenario-label">
            {roomIsOn ? 'System Off' : 'System On'}
          </span>
          <span className="home-view__scenario-sub">
            {roomIsOn ? 'Hold for 5 seconds to power down the room' : 'Press to power on the room'}
          </span>
        </button>
      </section>

      {/* ── Quick-access status cards ────────────────────────────────────── */}
      <section className="home-view__quick-access" aria-label="Quick access">
        {/* Master Volume quick card – swipe left/right to adjust */}
        <div
          className={`home-view__quick-card home-view__quick-card--vol${isDraggingVol ? ' home-view__quick-card--sliding' : ''}`}
          aria-label={`Master volume: ${masterVolPct}%`}
          role="slider"
          aria-valuenow={masterVolPct}
          aria-valuemin={0}
          aria-valuemax={100}
          aria-valuetext={`${masterVolPct}%`}
          tabIndex={0}
          onPointerDown={handleVolPointerDown}
          onPointerMove={handleVolPointerMove}
          onPointerUp={stopVolDrag}
          onPointerCancel={stopVolDrag}
        >
          <span className="home-view__quick-card-label">Master Volume</span>
          <span className="home-view__quick-card-value">{masterVolPct}%</span>
          <div
            className="home-view__vol-bar"
            role="presentation"
            aria-hidden="true"
          >
            <div
              className="home-view__vol-bar-fill"
              style={{ width: `${masterVolPct}%` }}
            />
          </div>
        </div>

        {/* Microphone Mute quick card */}
        <button
          className={`home-view__quick-card home-view__quick-card--btn${privacyMuted ? ' home-view__quick-card--muted' : ''}`}
          onClick={sendPrivacyMute}
          aria-pressed={privacyMuted}
          aria-label={privacyMuted ? 'Microphone Muted – tap to unmute' : 'Microphone Active – tap to mute'}
        >
          {!privacyMuted && (
            <span className="home-view__mic-active-dot" aria-hidden="true" />
          )}
          <span className="home-view__quick-card-icon" aria-hidden="true">
            {privacyMuted ? <MicOff size={24} /> : <Mic size={24} />}
          </span>
          <span className="home-view__quick-card-label">Microphone</span>
          <span className="home-view__quick-card-value">
            {privacyMuted ? 'MUTED' : 'ACTIVE'}
          </span>
        </button>
      </section>

    </div>
  );
};

export default HomeView;
